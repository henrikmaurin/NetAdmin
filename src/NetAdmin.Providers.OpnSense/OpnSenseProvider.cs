using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;
using NetAdmin.Providers.OpnSense.Models;

namespace NetAdmin.Providers.OpnSense;

public class OpnSenseProvider(
    HttpClient httpClient,
    IOptions<OpnSenseOptions> options,
    IEncryptedSettingsService settingsService,
    ILogger<OpnSenseProvider> logger) : IRoutingProvider, IDhcpProvider
{
    private readonly OpnSenseOptions _options = options.Value;

    public string ProviderName => "OPNsense";

    private async Task<string> PrepareClientAndGetBaseUrlAsync(CancellationToken ct)
    {
        var baseUrl = await settingsService.GetSettingAsync("OpnSense:BaseUrl", "OpnSense:BaseUrl", ct);
        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = _options.BaseUrl;

        var apiKey = await settingsService.GetSettingAsync("OpnSense:ApiKey", "OpnSense:ApiKey", ct);
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = _options.ApiKey;

        var apiSecret = await settingsService.GetSettingAsync("OpnSense:ApiSecret", "OpnSense:ApiSecret", ct);
        if (string.IsNullOrWhiteSpace(apiSecret))
            apiSecret = _options.ApiSecret;

        if (!string.IsNullOrWhiteSpace(apiKey) && !string.IsNullOrWhiteSpace(apiSecret))
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        }
        else
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
        }

        return baseUrl.TrimEnd('/');
    }

    public async Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        try
        {
            var baseUrl = await PrepareClientAndGetBaseUrlAsync(ct);
            var response = await httpClient.GetAsync($"{baseUrl}/api/core/firmware/running", ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunde inte ansluta till OPNsense API");
            return false;
        }
    }

    public async Task<IReadOnlyList<VlanDefinition>> GetVlansAsync(CancellationToken ct = default)
    {
        try
        {
            var baseUrl = await PrepareClientAndGetBaseUrlAsync(ct);
            var response = await httpClient.GetFromJsonAsync<OpnSenseSearchResponse<OpnSenseVlanRow>>(
                $"{baseUrl}/api/interfaces/vlan_settings/searchItem", ct);

            if (response?.Rows is null)
                return [];

            return response.Rows
                .OrderBy(v => v.VlanTag)
                .Select(v => new VlanDefinition(
                    VlanId: v.VlanTag,
                    Name: string.IsNullOrWhiteSpace(v.Description) ? $"VLAN {v.VlanTag}" : v.Description,
                    ParentInterface: v.ParentInterface ?? "okänd",
                    SubnetCidr: null))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fel vid hämtning av VLAN från OPNsense.");
            return [];
        }
    }

    public async Task<IReadOnlyList<ArpEntry>> GetArpTableAsync(CancellationToken ct = default)
    {
        try
        {
            var baseUrl = await PrepareClientAndGetBaseUrlAsync(ct);
            // getArp returnerar en ren JSON-array direkt utan "rows"-envelope
            var arpList = await httpClient.GetFromJsonAsync<List<OpnSenseArpDto>>(
                $"{baseUrl}/api/diagnostics/interface/getArp", ct);

            if (arpList is null)
                return [];

            return arpList
                .Where(a => !a.Expired && !string.IsNullOrWhiteSpace(a.Mac) && !string.IsNullOrWhiteSpace(a.Ip))
                .Select(a => new ArpEntry(
                    MacAddress: MacAddress.Normalize(a.Mac),
                    IpAddress: a.Ip!,
                    InterfaceName: !string.IsNullOrWhiteSpace(a.InterfaceDescription)
                        ? a.InterfaceDescription
                        : (a.InterfaceId ?? "Okänt"),
                    IsPermanent: a.Permanent))
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fel vid hämtning av ARP-tabell från OPNsense.");
            return [];
        }
    }

    public async Task<IReadOnlyList<NetworkInterface>> GetInterfacesAsync(CancellationToken ct = default)
    {
        // Bygger en snabb översikt baserat på de aktiva gränssnitten i ARP + VLAN
        var vlans = await GetVlansAsync(ct);
        return vlans.Select(v => new NetworkInterface(
            Identifier: $"{v.ParentInterface}_vlan{v.VlanId}",
            Name: v.Name,
            IpAddress: null,
            SubnetPrefix: null,
            VlanTag: v.VlanId,
            IsUp: true)).ToList();
    }

    public async Task<IReadOnlyList<DhcpLease>> GetLeasesAsync(CancellationToken ct = default)
    {
        var leases = new List<DhcpLease>();
        var baseUrl = await PrepareClientAndGetBaseUrlAsync(ct);

        // 1. Försök hämta från Kea DHCPv4 först (standard i nyare OPNsense)
        try
        {
            var keaResponse = await httpClient.GetFromJsonAsync<OpnSenseSearchResponse<OpnSenseKeaLeaseRow>>(
                $"{baseUrl}/api/kea/leases4/search", ct);

            if (keaResponse?.Rows is { Count: > 0 })
            {
                leases.AddRange(keaResponse.Rows
                    .Where(r => !string.IsNullOrWhiteSpace(r.MacAddress) && !string.IsNullOrWhiteSpace(r.IpAddress))
                    .Select(r => new DhcpLease(
                        MacAddress: MacAddress.Normalize(r.MacAddress),
                        IpAddress: r.IpAddress!,
                        Hostname: NullIfEmpty(r.Hostname?.TrimEnd('.')),
                        IsStatic: false,
                        ExpiresAt: r.ExpireUnix > 0
                            ? DateTimeOffset.FromUnixTimeSeconds(r.ExpireUnix.Value)
                            : null)));
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Kea DHCPv4 svarade inte eller används ej, provar ISC DHCPv4.");
        }

        // 2. Fallback / komplement: ISC DHCPv4 om Kea inte gav några träffar
        if (leases.Count == 0)
        {
            try
            {
                var iscResponse = await httpClient.GetFromJsonAsync<OpnSenseSearchResponse<OpnSenseIscLeaseRow>>(
                    $"{baseUrl}/api/dhcpv4/leases/searchLease", ct);

                if (iscResponse?.Rows is { Count: > 0 })
                {
                    leases.AddRange(iscResponse.Rows
                        .Where(r => !string.IsNullOrWhiteSpace(r.MacAddress) && !string.IsNullOrWhiteSpace(r.IpAddress))
                        .Select(r => new DhcpLease(
                            MacAddress: MacAddress.Normalize(r.MacAddress),
                            IpAddress: r.IpAddress!,
                            Hostname: NullIfEmpty(r.Hostname) ?? NullIfEmpty(r.Description),
                            IsStatic: string.Equals(r.LeaseType, "static", StringComparison.OrdinalIgnoreCase),
                            ExpiresAt: null)));
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, "ISC DHCPv4 svarade inte eller används ej.");
            }
        }

        return leases;
    }

    public async Task<bool> AddStaticLeaseAsync(
        string ipAddress,
        string macAddress,
        string hostname,
        string? description = null,
        CancellationToken ct = default)
    {
        try
        {
            var baseUrl = await PrepareClientAndGetBaseUrlAsync(ct);

            // Sanera hostname så att OPNsense godkänner det i DNS (endast a-z, 0-9 och bindestreck)
            var safeHostname = SanitizeHostname(hostname);
            var descr = description ?? $"{hostname} (Synkad från NetAdmin)";

            // 1. Försök med Kea DHCPv4 (OPNsense moderna DHCP-server)
            var subnetsResp = await httpClient.GetAsync($"{baseUrl}/api/kea/dhcpv4/search_subnet", ct);
            if (subnetsResp.IsSuccessStatusCode)
            {
                using var doc = await JsonDocument.ParseAsync(await subnetsResp.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
                if (doc.RootElement.TryGetProperty("rows", out var rows) && rows.GetArrayLength() > 0)
                {
                    // Matcha rätt subnät baserat på de första tre oktetterna (t.ex. "192.168.1.")
                    var ipPrefix = string.Join('.', ipAddress.Split('.').Take(3)) + ".";
                    string? subnetUuid = null;

                    foreach (var row in rows.EnumerateArray())
                    {
                        var subnetStr = row.TryGetProperty("subnet", out var s) ? s.GetString() : null;
                        if (subnetStr is not null && subnetStr.StartsWith(ipPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            subnetUuid = row.TryGetProperty("uuid", out var u) ? u.GetString() : null;
                            break;
                        }
                    }

                    subnetUuid ??= rows[0].TryGetProperty("uuid", out var firstUuid) ? firstUuid.GetString() : null;

                    if (!string.IsNullOrWhiteSpace(subnetUuid))
                    {
                        var keaPayload = new
                        {
                            reservation = new
                            {
                                subnet = subnetUuid,
                                ip_address = ipAddress,
                                hw_address = macAddress.ToLowerInvariant(),
                                hostname = safeHostname,
                                description = descr
                            }
                        };

                        var addResp = await httpClient.PostAsJsonAsync($"{baseUrl}/api/kea/dhcpv4/add_reservation", keaPayload, ct);
                        if (addResp.IsSuccessStatusCode)
                        {
                            // Verkställ konfigurationen i Kea-tjänsten
                            await httpClient.PostAsync($"{baseUrl}/api/kea/service/reconfigure", null, ct);
                            return true;
                        }
                    }
                }
            }

            // 2. Fallback till klassisk ISC DHCPv4
            var iscPayload = new
            {
                lease = new
                {
                    ipaddr = ipAddress,
                    mac = macAddress.ToLowerInvariant(),
                    hostname = safeHostname,
                    descr = descr
                }
            };

            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/dhcpv4/settings/add_static", iscPayload, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunde inte synka namn/statisk DHCP-lease för {Ip} ({Hostname})", ipAddress, hostname);
            return false;
        }
    }

    private static string SanitizeHostname(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "netadmin-host";

        var normalized = input.Trim().ToLowerInvariant()
            .Replace("å", "a").Replace("ä", "a").Replace("ö", "o")
            .Replace(" ", "-").Replace("_", "-");

        var chars = normalized.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray();
        var result = new string(chars).Trim('-');
        return string.IsNullOrWhiteSpace(result) ? "netadmin-host" : result;
    }

    public async Task<bool> BlockHostAsync(string ipOrMac, CancellationToken ct = default)
    {
        try
        {
            // OPNsense API-anrop för att lägga till en IP/MAC i brandväggens block-alias eller regelverk
            var payload = new
            {
                item = new
                {
                    content = ipOrMac,
                    enabled = "1",
                    descr = $"Blockerad via NetAdmin {DateTime.UtcNow:yyyy-MM-dd}"
                }
            };

            var response = await httpClient.PostAsJsonAsync("/api/firewall/alias_util/add/BlockedHosts", payload, ct);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunde inte blockera värd {Host} i OPNsense brandvägg", ipOrMac);
            return false;
        }
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}