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

    // Adds a camera entry to a go2rtc instance so Frigate can use it as a source.
    // This is intentionally simple: it will POST { name, url } to a presumed
    // go2rtc endpoint /api/sources and DELETE /api/sources/{name} for removal.
    // The concrete go2rtc URL can be provided or resolved from settings
    // (device setting "go2rtc:baseurl" or global "Go2Rtc:BaseUrl").
    public async Task<bool> AddCameraViaGo2RtcAsync(string? deviceId, string cameraName, string rtspUrl, string? go2rtcBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "go2rtc:baseurl", "Go2Rtc:BaseUrl", go2rtcBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 1984); // default go2rtc port (commonly 1984)
        if (string.IsNullOrWhiteSpace(baseUrl)) return false;

        try
        {
            var payload = JsonSerializer.Serialize(new { name = cameraName, url = rtspUrl });
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/sources");
            req.Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            using var resp = await HttpClient.SendAsync(req, ct);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveCameraViaGo2RtcAsync(string? deviceId, string cameraName, string? go2rtcBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "go2rtc:baseurl", "Go2Rtc:BaseUrl", go2rtcBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 1984);
        if (string.IsNullOrWhiteSpace(baseUrl)) return false;

        try
        {
            var safeName = Uri.EscapeDataString(cameraName);
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/sources/{safeName}");
            using var resp = await HttpClient.SendAsync(req, ct);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddCameraToFrigateAsync(string? deviceId, string cameraName, string rtspUrl, string? frigateBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "Frigate:BaseUrl", frigateBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 5000);
        if (string.IsNullOrWhiteSpace(baseUrl)) return false;

        var apiToken = await ResolveSettingAsync(deviceId, "api:token", "Frigate:ApiToken", null, ct);

        try
        {
            // Fetch current config
            using var getReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: apiToken);
            using var cfgDoc = await SendAndParseJsonAsync(getReq, ct);
            if (cfgDoc is null) return false;

            var root = cfgDoc.RootElement.Clone();

            // We will construct a mutable JSON object using a dictionary and then serialize.
            var cameras = new Dictionary<string, object>();

            if (root.TryGetProperty("cameras", out var camsProp) && camsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var cam in camsProp.EnumerateObject())
                {
                    cameras[cam.Name] = JsonSerializer.Deserialize<object>(cam.Value.GetRawText())!;
                }
            }

            // Minimal camera definition that Frigate understands: ffmpeg inputs with path.
            var newCam = new Dictionary<string, object>()
            {
                ["ffmpeg"] = new Dictionary<string, object>
                {
                    ["inputs"] = new[] { new Dictionary<string, object> { ["path"] = rtspUrl, ["roles"] = new[] { "detect", "record", "rtmp" } } }
                },
                ["detect"] = new Dictionary<string, object> { ["enabled"] = true }
            };

            cameras[cameraName] = newCam;

            var payload = new Dictionary<string, object>
            {
                ["cameras"] = cameras
            };

            var json = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/config");
            if (!string.IsNullOrWhiteSpace(apiToken))
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            req.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var resp = await HttpClient.SendAsync(req, ct);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveCameraFromFrigateAsync(string? deviceId, string cameraName, string? frigateBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "Frigate:BaseUrl", frigateBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 5000);
        if (string.IsNullOrWhiteSpace(baseUrl)) return false;

        var apiToken = await ResolveSettingAsync(deviceId, "api:token", "Frigate:ApiToken", null, ct);

        try
        {
            using var getReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: apiToken);
            using var cfgDoc = await SendAndParseJsonAsync(getReq, ct);
            if (cfgDoc is null) return false;

            var root = cfgDoc.RootElement.Clone();

            var cameras = new Dictionary<string, object>();
            if (root.TryGetProperty("cameras", out var camsProp) && camsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var cam in camsProp.EnumerateObject())
                {
                    if (!string.Equals(cam.Name, cameraName, StringComparison.OrdinalIgnoreCase))
                        cameras[cam.Name] = JsonSerializer.Deserialize<object>(cam.Value.GetRawText())!;
                }
            }

            var payload = new Dictionary<string, object>
            {
                ["cameras"] = cameras
            };

            var json = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/config");
            if (!string.IsNullOrWhiteSpace(apiToken))
                req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
            req.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var resp = await HttpClient.SendAsync(req, ct);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IReadOnlyDictionary<string, string?>> GetCameraSourcesAsync(string? deviceId = null, string? frigateBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "Frigate:BaseUrl", frigateBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 5000);
        if (string.IsNullOrWhiteSpace(baseUrl)) return new Dictionary<string, string?>();

        var apiToken = await ResolveSettingAsync(deviceId, "api:token", "Frigate:ApiToken", null, ct);

        try
        {
            using var getReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: apiToken);
            using var cfgDoc = await SendAndParseJsonAsync(getReq, ct);
            if (cfgDoc is null) return new Dictionary<string, string?>();

            var root = cfgDoc.RootElement;
            var result = new Dictionary<string, string?>();

            if (root.TryGetProperty("cameras", out var camsProp) && camsProp.ValueKind == JsonValueKind.Object)
            {
                foreach (var cam in camsProp.EnumerateObject())
                {
                    string? path = null;
                    if (cam.Value.ValueKind == JsonValueKind.Object && cam.Value.TryGetProperty("ffmpeg", out var ff) && ff.ValueKind == JsonValueKind.Object)
                    {
                        if (ff.TryGetProperty("inputs", out var inputs) && inputs.ValueKind == JsonValueKind.Array && inputs.GetArrayLength() > 0)
                        {
                            var first = inputs[0];
                            if (first.ValueKind == JsonValueKind.Object && first.TryGetProperty("path", out var p) && p.ValueKind == JsonValueKind.String)
                            {
                                path = p.GetString();
                            }
                        }
                    }

                    result[cam.Name] = path;
                }
            }

            return result;
        }
        catch
        {
            return new Dictionary<string, string?>();
        }
    }

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default)
    {
        var sum = await GetStatusAsync(serviceId, baseUrl, ct);
        if (sum is null) return null;

        var summary = $"Cameras={sum.CameraCount}; Detectors={string.Join(',', sum.Detectors)}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, "RUNNING", sum.Version, summary, sum);
    }
}
