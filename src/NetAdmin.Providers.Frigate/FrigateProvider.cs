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
    public IReadOnlyCollection<NetAdmin.Core.Models.ServiceType> SupportedServiceTypes => [NetAdmin.Core.Models.ServiceType.Nvr];

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

    public async Task<IReadOnlyList<FrigateCameraDefinition>> GetCameraDefinitionsAsync(string? deviceId = null, string? frigateBaseUrl = null, CancellationToken ct = default)
    {
        var baseUrlRaw = await ResolveSettingAsync(deviceId, "api:baseurl", "Frigate:BaseUrl", frigateBaseUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 5000);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return [];

        var apiToken = await ResolveSettingAsync(deviceId, "api:token", "Frigate:ApiToken", null, ct);
        using var request = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: apiToken);
        using var config = await SendAndParseJsonAsync(request, ct);
        if (config is null || !config.RootElement.TryGetProperty("cameras", out var cameras) || cameras.ValueKind != JsonValueKind.Object)
            return [];

        var definitions = new List<FrigateCameraDefinition>();
        foreach (var camera in cameras.EnumerateObject())
        {
            var streams = new List<NetAdmin.Core.Models.CameraStreamDefinition>();
            if (camera.Value.ValueKind == JsonValueKind.Object &&
                camera.Value.TryGetProperty("ffmpeg", out var ffmpeg) && ffmpeg.ValueKind == JsonValueKind.Object &&
                ffmpeg.TryGetProperty("inputs", out var inputs) && inputs.ValueKind == JsonValueKind.Array)
            {
                var streamNumber = 0;
                foreach (var input in inputs.EnumerateArray())
                {
                    if (input.ValueKind != JsonValueKind.Object ||
                        !input.TryGetProperty("path", out var pathProperty) || pathProperty.ValueKind != JsonValueKind.String)
                        continue;

                    var path = pathProperty.GetString();
                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    streamNumber++;
                    var roles = input.TryGetProperty("roles", out var rolesProperty) && rolesProperty.ValueKind == JsonValueKind.Array
                        ? string.Join(", ", rolesProperty.EnumerateArray()
                            .Where(role => role.ValueKind == JsonValueKind.String)
                            .Select(role => role.GetString())
                            .Where(role => !string.IsNullOrWhiteSpace(role)))
                        : string.Empty;
                    var streamName = string.IsNullOrWhiteSpace(roles)
                        ? $"Stream {streamNumber}"
                        : $"Stream {streamNumber} ({roles})";
                    streams.Add(new NetAdmin.Core.Models.CameraStreamDefinition(
                        streamName,
                        path,
                        GetStreamProtocol(path)));
                }
            }

            definitions.Add(new FrigateCameraDefinition(camera.Name, streams));
        }

        return definitions;
    }

    public async Task<IReadOnlyDictionary<string, string?>> GetCameraSourcesAsync(string? deviceId = null, string? frigateBaseUrl = null, CancellationToken ct = default)
    {
        var definitions = await GetCameraDefinitionsAsync(deviceId, frigateBaseUrl, ct);
        return definitions.ToDictionary(
            definition => definition.Name,
            definition => definition.Streams.FirstOrDefault()?.Url,
            StringComparer.OrdinalIgnoreCase);
    }

    private static string GetStreamProtocol(string path) =>
        Uri.TryCreate(path, UriKind.Absolute, out var uri) && !string.IsNullOrWhiteSpace(uri.Scheme)
            ? uri.Scheme.ToUpperInvariant()
            : "RTSP";

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default)
    {
        var sum = await GetStatusAsync(linkedDeviceId ?? serviceId, baseUrl, ct);
        if (sum is null) return null;

        var summary = $"Cameras={sum.CameraCount}; Detectors={string.Join(',', sum.Detectors)}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, "RUNNING", sum.Version, summary, sum);
    }
}
