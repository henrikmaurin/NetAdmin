using System.Text.Json;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;

namespace NetAdmin.Providers.HomeAssistant;

public class HomeAssistantProvider(
    HttpClient httpClient,
    IEncryptedSettingsService settingsService)
    : HttpIntegrationProviderBase(httpClient, settingsService), IHomeAssistantProvider, NetAdmin.Core.Providers.IServiceIntegration
{
    public override string ProviderName => "Home Assistant";

    public override async Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        var status = await GetStatusAsync(ct: ct);
        return status is not null;
    }

    public async Task<HomeAssistantStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "HomeAssistant:BaseUrl", ipOrUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 8123);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return null;

        var accessToken = await ResolveSettingAsync(deviceId, "api:token", "HomeAssistant:AccessToken", null, ct);

        using var configReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: accessToken);
        using var configDoc = await SendAndParseJsonAsync(configReq, ct);
        if (configDoc is null)
            return null;

        var root = configDoc.RootElement;
        var version = root.TryGetProperty("version", out var verProp) && verProp.ValueKind == JsonValueKind.String
            ? verProp.GetString() ?? "Unknown"
            : "Unknown";

        var locationName = root.TryGetProperty("location_name", out var locProp) && locProp.ValueKind == JsonValueKind.String
            ? locProp.GetString() ?? "Home"
            : "Home";

        var state = root.TryGetProperty("state", out var stProp) && stProp.ValueKind == JsonValueKind.String
            ? stProp.GetString() ?? "RUNNING"
            : "RUNNING";

        var componentCount = root.TryGetProperty("components", out var compProp) && compProp.ValueKind == JsonValueKind.Array
            ? compProp.GetArrayLength()
            : 0;

        var entityCount = 0;
        using var statesReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/states", bearerToken: accessToken);
        using var statesDoc = await SendAndParseJsonAsync(statesReq, ct);
        if (statesDoc is not null && statesDoc.RootElement.ValueKind == JsonValueKind.Array)
        {
            entityCount = statesDoc.RootElement.GetArrayLength();
        }

        return new HomeAssistantStatusSummary(
            Version: version,
            LocationName: locationName,
            State: state,
            ComponentCount: componentCount,
            EntityCount: entityCount);
    }

    public async Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantEntity>> GetEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "HomeAssistant:BaseUrl", ipOrUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 8123);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return Array.Empty<NetAdmin.Core.Models.HomeAssistantEntity>();

        var accessToken = await ResolveSettingAsync(deviceId, "api:token", "HomeAssistant:AccessToken", null, ct);

        using var statesReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/states", bearerToken: accessToken);
        using var statesDoc = await SendAndParseJsonAsync(statesReq, ct);
        if (statesDoc is null || statesDoc.RootElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<NetAdmin.Core.Models.HomeAssistantEntity>();

        // Try to fetch device registry to enrich candidate identifiers
        var devicesById = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        try
        {
            using var devicesReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/devices", bearerToken: accessToken);
            using var devicesDoc = await SendAndParseJsonAsync(devicesReq, ct);
            if (devicesDoc is not null && devicesDoc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var d in devicesDoc.RootElement.EnumerateArray())
                {
                    if (d.ValueKind != JsonValueKind.Object) continue;
                    if (d.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.String)
                    {
                        var id = idProp.GetString()!;
                        devicesById[id] = d;
                    }
                }
            }
        }
        catch
        {
            // Best effort: ignore failures to fetch devices
        }

        var list = new List<NetAdmin.Core.Models.HomeAssistantEntity>();
        foreach (var item in statesDoc.RootElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object) continue;

            var entityId = item.GetProperty("entity_id").GetString() ?? string.Empty;
            var state = item.GetProperty("state").GetString() ?? string.Empty;

            string? friendly = null;
            string? deviceClass = null;
            string? uom = null;

            var candidates = new List<string>();

            if (item.TryGetProperty("attributes", out var attr) && attr.ValueKind == JsonValueKind.Object)
            {
                // Extract common fields
                if (attr.TryGetProperty("friendly_name", out var fn) && fn.ValueKind == JsonValueKind.String)
                    friendly = fn.GetString();

                if (attr.TryGetProperty("device_class", out var dc) && dc.ValueKind == JsonValueKind.String)
                    deviceClass = dc.GetString();

                if (attr.TryGetProperty("unit_of_measurement", out var um) && um.ValueKind == JsonValueKind.String)
                    uom = um.GetString();

                // Collect candidate identifiers (ip, ip_address, mac, mac_address, device_id)
                foreach (var p in attr.EnumerateObject())
                {
                    var key = p.Name.ToLowerInvariant();
                    if (key.Contains("ip") || key.Contains("mac") || key.Contains("device_id") || key.Contains("mac_address") || key.Contains("ip_address"))
                    {
                        try
                        {
                            if (p.Value.ValueKind == JsonValueKind.String)
                            {
                                var v = p.Value.GetString();
                                if (!string.IsNullOrWhiteSpace(v)) candidates.Add(v!);
                            }
                            else
                            {
                                // fallback to serialize non-string values
                                candidates.Add(p.Value.ToString());
                            }
                        }
                        catch { /* ignore parsing errors */ }
                    }
                }

                // If the entity references a device_id, try to enrich candidates from the device registry
                if (attr.TryGetProperty("device_id", out var did) && did.ValueKind == JsonValueKind.String)
                {
                    var devId = did.GetString();
                    if (!string.IsNullOrWhiteSpace(devId) && devicesById.TryGetValue(devId!, out var devEl))
                    {
                        // connections: array of [type, value] pairs, e.g. [ ["mac", "AA:BB:..."], ["ipv4", "192.168.1.2"] ]
                        if (devEl.TryGetProperty("connections", out var con) && con.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var c in con.EnumerateArray())
                            {
                                if (c.ValueKind == JsonValueKind.Array && c.GetArrayLength() >= 2)
                                {
                                    try
                                    {
                                        var val = c[1].GetString();
                                        if (!string.IsNullOrWhiteSpace(val)) candidates.Add(val!);
                                    }
                                    catch { }
                                }
                            }
                        }

                        // identifiers: array of [domain, identifier]
                        if (devEl.TryGetProperty("identifiers", out var ids) && ids.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var idv in ids.EnumerateArray())
                            {
                                if (idv.ValueKind == JsonValueKind.Array && idv.GetArrayLength() >= 2)
                                {
                                    try
                                    {
                                        var val = idv[1].GetString();
                                        if (!string.IsNullOrWhiteSpace(val)) candidates.Add(val!);
                                    }
                                    catch { }
                                }
                                else if (idv.ValueKind == JsonValueKind.String)
                                {
                                    var val = idv.GetString();
                                    if (!string.IsNullOrWhiteSpace(val)) candidates.Add(val!);
                                }
                            }
                        }
                    }
                }
            }

            // Serialize attributes to JSON for consumers
            var attributesJson = item.TryGetProperty("attributes", out var a) ? a.GetRawText() : "{}";

            list.Add(new NetAdmin.Core.Models.HomeAssistantEntity(
                EntityId: entityId,
                State: state,
                FriendlyName: friendly,
                DeviceClass: deviceClass,
                UnitOfMeasure: uom,
                AttributesJson: attributesJson,
                CandidateDeviceIdentifiers: candidates.Distinct().ToList()));
        }

        return list;
    }

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default)
    {
        var sum = await GetStatusAsync(serviceId, baseUrl, ct);
        if (sum is null) return null;

        var summary = $"Location={sum.LocationName}; Entities={sum.EntityCount}; Components={sum.ComponentCount}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, sum.State, sum.Version, summary, sum);
    }
}
