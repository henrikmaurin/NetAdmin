using System.Text.Json;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;

namespace NetAdmin.Providers.Frigate;

public class FrigateProvider(
    HttpClient httpClient,
    IEncryptedSettingsService settingsService)
    : HttpIntegrationProviderBase(httpClient, settingsService), IFrigateProvider, NetAdmin.Core.Providers.IServiceIntegration
{
    public override string ProviderName => "Frigate NVR";

    public override async Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        var status = await GetStatusAsync(ct: ct);
        return status is not null;
    }

    public async Task<FrigateStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "Frigate:BaseUrl", ipOrUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 5000);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return null;

        var apiToken = await ResolveSettingAsync(deviceId, "api:token", "Frigate:ApiToken", null, ct);

        using var statsReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/stats", bearerToken: apiToken);
        using var statsDoc = await SendAndParseJsonAsync(statsReq, ct);
        if (statsDoc is null)
            return null;

        var root = statsDoc.RootElement;

        var version = "Unknown";
        if (root.TryGetProperty("service", out var serviceProp) &&
            serviceProp.ValueKind == JsonValueKind.Object &&
            serviceProp.TryGetProperty("version", out var verProp) &&
            verProp.ValueKind == JsonValueKind.String)
        {
            version = verProp.GetString() ?? "Unknown";
        }

        var detectors = new List<string>();
        if (root.TryGetProperty("detectors", out var detProp) && detProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var det in detProp.EnumerateObject())
            {
                detectors.Add(det.Name);
            }
        }

        var cameras = new List<FrigateCameraSummary>();
        if (root.TryGetProperty("cameras", out var camsProp) && camsProp.ValueKind == JsonValueKind.Object)
        {
            foreach (var cam in camsProp.EnumerateObject())
            {
                double camFps = 0;
                double detFps = 0;
                bool detEnabled = true;

                if (cam.Value.ValueKind == JsonValueKind.Object)
                {
                    if (cam.Value.TryGetProperty("camera_fps", out var cf) && cf.TryGetDouble(out var cfVal))
                        camFps = cfVal;
                    if (cam.Value.TryGetProperty("detection_fps", out var df) && df.TryGetDouble(out var dfVal))
                        detFps = dfVal;
                    if (cam.Value.TryGetProperty("detection_enabled", out var de) &&
                        (de.ValueKind is JsonValueKind.True or JsonValueKind.False))
                        detEnabled = de.GetBoolean();
                }

                cameras.Add(new FrigateCameraSummary(cam.Name, camFps, detFps, detEnabled));
            }
        }

        return new FrigateStatusSummary(
            Version: version,
            CameraCount: cameras.Count,
            Detectors: detectors,
            Cameras: cameras);
    }

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default)
    {
        var sum = await GetStatusAsync(serviceId, baseUrl, ct);
        if (sum is null) return null;

        var summary = $"Cameras={sum.CameraCount}; Detectors={string.Join(',', sum.Detectors)}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, "RUNNING", sum.Version, summary, sum);
    }
}
