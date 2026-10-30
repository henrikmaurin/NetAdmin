using System.Net;
using System.Net.Http.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;
using NetAdmin.Providers.UniFi.Models;

namespace NetAdmin.Providers.UniFi;

public class UniFiProvider(
    HttpClient httpClient,
    IOptions<UniFiOptions> options,
    IEncryptedSettingsService settingsService,
    ILogger<UniFiProvider> logger) : IWirelessProvider
{
    private readonly UniFiOptions _options = options.Value;
    private bool _isLoggedIn;
    private string? _lastAuthFingerprint;

    public string ProviderName => "UniFi Network";

    private record ResolvedUniFiSettings(
        string BaseUrl,
        string Site,
        string? ApiKey,
        string? Username,
        string? Password,
        bool IsUniFiOs)
    {
        public string ApiPrefix => IsUniFiOs
            ? $"{BaseUrl}/proxy/network/api/s/{Site}"
            : $"{BaseUrl}/api/s/{Site}";
    }

    private async Task<ResolvedUniFiSettings> ResolveSettingsAsync(CancellationToken ct)
    {
        var baseUrl = await settingsService.GetSettingAsync("UniFi:BaseUrl", "UniFi:BaseUrl", ct);
        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = _options.BaseUrl;

        var site = await settingsService.GetSettingAsync("UniFi:Site", "UniFi:Site", ct);
        if (string.IsNullOrWhiteSpace(site))
            site = _options.Site;

        var apiKey = await settingsService.GetSettingAsync("UniFi:ApiKey", "UniFi:ApiKey", ct);
        if (string.IsNullOrWhiteSpace(apiKey))
            apiKey = _options.ApiKey;

        var username = await settingsService.GetSettingAsync("UniFi:Username", "UniFi:Username", ct);
        if (string.IsNullOrWhiteSpace(username))
            username = _options.Username;

        var password = await settingsService.GetSettingAsync("UniFi:Password", "UniFi:Password", ct);
        if (string.IsNullOrWhiteSpace(password))
            password = _options.Password;

        var isUniFiOsRaw = await settingsService.GetSettingAsync("UniFi:IsUniFiOs", "UniFi:IsUniFiOs", ct);
        var isUniFiOs = bool.TryParse(isUniFiOsRaw, out var parsed) ? parsed : _options.IsUniFiOs;

        httpClient.DefaultRequestHeaders.Remove("X-API-KEY");
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        }

        var fingerprint = $"{baseUrl}|{site}|{apiKey}|{username}|{password}|{isUniFiOs}";
        if (_lastAuthFingerprint != fingerprint)
        {
            _isLoggedIn = false;
            _lastAuthFingerprint = fingerprint;
        }

        return new ResolvedUniFiSettings(
            BaseUrl: baseUrl.TrimEnd('/'),
            Site: site,
            ApiKey: apiKey,
            Username: username,
            Password: password,
            IsUniFiOs: isUniFiOs);
    }

    private async Task<ResolvedUniFiSettings> EnsureAuthenticatedAsync(CancellationToken ct)
    {
        var cfg = await ResolveSettingsAsync(ct);

        if (!string.IsNullOrWhiteSpace(cfg.ApiKey) || _isLoggedIn)
            return cfg;

        if (string.IsNullOrWhiteSpace(cfg.Username) || string.IsNullOrWhiteSpace(cfg.Password))
            return cfg;

        var loginEndpoint = cfg.IsUniFiOs ? $"{cfg.BaseUrl}/api/auth/login" : $"{cfg.BaseUrl}/api/login";
        var payload = new { username = cfg.Username, password = cfg.Password };

        var response = await httpClient.PostAsJsonAsync(loginEndpoint, payload, ct);
        response.EnsureSuccessStatusCode();
        _isLoggedIn = true;
        return cfg;
    }

    private async Task<List<T>> GetUniFiDataAsync<T>(string relativeEndpoint, CancellationToken ct)
    {
        var cfg = await EnsureAuthenticatedAsync(ct);

        var url = $"{cfg.ApiPrefix}/{relativeEndpoint.TrimStart('/')}";
        var response = await httpClient.GetAsync(url, ct);

        // Om sessionen gått ut (401), logga in på nytt en gång
        if (response.StatusCode == HttpStatusCode.Unauthorized && string.IsNullOrWhiteSpace(cfg.ApiKey))
        {
            _isLoggedIn = false;
            cfg = await EnsureAuthenticatedAsync(ct);
            response = await httpClient.GetAsync(url, ct);
        }

        response.EnsureSuccessStatusCode();
        var wrapper = await response.Content.ReadFromJsonAsync<UniFiResponse<T>>(cancellationToken: ct);
        return wrapper?.Data ?? [];
    }

    public async Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        try
        {
            await GetUniFiDataAsync<UniFiDeviceDto>("stat/device-basic", ct);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunde inte ansluta till UniFi Controller på {BaseUrl}", _options.BaseUrl);
            return false;
        }
    }

    public async Task<IReadOnlyList<AccessPoint>> GetAccessPointsAsync(CancellationToken ct = default)
    {
        try
        {
            var devices = await GetUniFiDataAsync<UniFiDeviceDto>("stat/device", ct);

            return devices
                .Where(d => string.Equals(d.Type, "uap", StringComparison.OrdinalIgnoreCase))
                .Select(d => new AccessPoint(
                    Id: d.Id ?? string.Empty,
                    Name: !string.IsNullOrWhiteSpace(d.Name) ? d.Name : (d.Model ?? "UniFi AP"),
                    MacAddress: MacAddress.Normalize(d.Mac),
                    IpAddress: d.Ip ?? "-",
                    Model: d.Model ?? "UAP",
                    IsOnline: d.State == 1,
                    ConnectedClientsCount: d.NumSta))
                .OrderBy(a => a.Name)
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fel vid hämtning av accesspunkter från UniFi.");
            return [];
        }
    }

    public async Task<IReadOnlyList<WirelessNetwork>> GetWirelessNetworksAsync(CancellationToken ct = default)
    {
        try
        {
            var wlansTask = GetUniFiDataAsync<UniFiWlanConfDto>("rest/wlanconf", ct);
            var networksTask = GetUniFiDataAsync<UniFiNetworkConfDto>("rest/networkconf", ct);

            await Task.WhenAll(wlansTask, networksTask);

            var vlanByNetworkId = networksTask.Result
                .Where(n => !string.IsNullOrWhiteSpace(n.Id))
                .ToDictionary(n => n.Id!, n => n.VlanEnabled ? n.VlanId : null);

            return wlansTask.Result
                .Select(w =>
                {
                    int? resolvedVlan = w.VlanEnabled ? w.LegacyVlanId : null;
                    if (resolvedVlan is null && !string.IsNullOrWhiteSpace(w.NetworkConfId))
                    {
                        vlanByNetworkId.TryGetValue(w.NetworkConfId, out resolvedVlan);
                    }

                    return new WirelessNetwork(
                        Id: w.Id ?? string.Empty,
                        Ssid: w.Ssid ?? "Okänt SSID",
                        VlanId: resolvedVlan,
                        IsEnabled: w.Enabled);
                })
                .OrderBy(w => w.Ssid)
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fel vid hämtning av SSID/WLAN från UniFi.");
            return [];
        }
    }

    public async Task<IReadOnlyList<WirelessClient>> GetWirelessClientsAsync(CancellationToken ct = default)
    {
        try
        {
            var clientsTask = GetUniFiDataAsync<UniFiClientDto>("stat/sta", ct);
            var apsTask = GetAccessPointsAsync(ct);

            await Task.WhenAll(clientsTask, apsTask);

            var apNameByMac = apsTask.Result
                .ToDictionary(ap => ap.MacAddress, ap => ap.Name);

            return clientsTask.Result
                .Where(c => !c.IsWired && !string.IsNullOrWhiteSpace(c.Mac))
                .Select(c =>
                {
                    var normalizedApMac = MacAddress.Normalize(c.ApMac);
                    apNameByMac.TryGetValue(normalizedApMac, out var apName);

                    return new WirelessClient(
                        MacAddress: MacAddress.Normalize(c.Mac),
                        Hostname: !string.IsNullOrWhiteSpace(c.AliasName) ? c.AliasName : c.Hostname,
                        ApMacAddress: normalizedApMac,
                        ApName: apName ?? normalizedApMac,
                        Ssid: c.Essid ?? "-",
                        SignalRssi: c.SignalDbm ?? c.Rssi,
                        RadioBand: FormatRadioBand(c.Radio, c.Channel));
                })
                .ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fel vid hämtning av trådlösa klienter från UniFi.");
            return [];
        }
    }

    public async Task<bool> RestartAccessPointAsync(string apMacAddress, CancellationToken ct = default)
    {
        try
        {
            var cfg = await EnsureAuthenticatedAsync(ct);

            // UniFi kommando-endpoint för enhet
            var normalizedMac = MacAddress.Normalize(apMacAddress).ToLowerInvariant();

            var response = await httpClient.PostAsJsonAsync($"{cfg.ApiPrefix}/cmd/dev/mgr", new
            {
                mac = normalizedMac,
                cmd = "restart"
            }, ct);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunde inte starta om UniFi AP med MAC {Mac}", apMacAddress);
            return false;
        }
    }

    private static string? FormatRadioBand(string? radio, int? channel)
    {
        var band = radio switch
        {
            "ng" => "2.4 GHz",
            "na" => "5 GHz",
            "6e" => "6 GHz",
            _ => null
        };

        return (band, channel) switch
        {
            (not null, not null) => $"{band} (Ch {channel})",
            (not null, null) => band,
            _ => null
        };
    }
}