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

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default)
    {
        var sum = await GetStatusAsync(serviceId, baseUrl, ct);
        if (sum is null) return null;

        var summary = $"Location={sum.LocationName}; Entities={sum.EntityCount}; Components={sum.ComponentCount}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, sum.State, sum.Version, summary, sum);
    }
}
