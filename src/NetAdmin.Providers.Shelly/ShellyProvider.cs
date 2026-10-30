using System.Net.Http.Json;
using System.Text.Json;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Shelly;

public class ShellyProvider(
    IShellyStore store,
    ProductCatalogRegistry catalog,
    HttpClient httpClient) : IShellyProvider
{
    public string ProviderName => "Shelly IoT Devices";

    public Task<bool> TestConnectionAsync(CancellationToken ct = default) => Task.FromResult(true);

    public Task<IReadOnlyList<ShellyDevice>> GetDevicesAsync(CancellationToken ct = default) =>
        store.GetDevicesAsync(ct);

    public Task SaveDeviceAsync(ShellyDevice device, CancellationToken ct = default) =>
        store.SaveDeviceAsync(device, ct);

    public Task DeleteDeviceAsync(string id, CancellationToken ct = default) =>
        store.DeleteDeviceAsync(id, ct);

    /// <summary>
    /// Identifierar en Shelly-enhet direkt över nätverket via http://{ip}/shelly (fungerar för Gen1, Gen2, Gen3, Gen4)
    /// eller /rpc/Shelly.GetDeviceInfo och matchar mot registrerade ShellyProductDefinition-klasser.
    /// </summary>
    public async Task<ShellyProbedInfo?> ProbeDeviceAsync(string ipAddress, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            return null;

        var cleanIp = ipAddress.Trim()
            .Replace("http://", "", StringComparison.OrdinalIgnoreCase)
            .Replace("https://", "", StringComparison.OrdinalIgnoreCase)
            .TrimEnd('/');

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(4));

        try
        {
            var url = $"http://{cleanIp}/shelly";
            using var response = await httpClient.GetAsync(url, cts.Token);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cts.Token);
            var root = doc.RootElement;

            // Gen2 / Gen3 / Gen4 format: { "name": "...", "id": "shellyplus1pm-...", "mac": "...", "model": "SNSW-001P16EU", "gen": 2, "fw_id": "...", "app": "Plus1PM" }
            // Gen1 format: { "type": "SHSW-1", "mac": "...", "auth": false, "fw": "...", "num_outputs": 1 }
            var gen = root.TryGetProperty("gen", out var genProp) && genProp.TryGetInt32(out var g) ? g : 1;

            var rawModel = root.TryGetProperty("model", out var modelProp) && modelProp.ValueKind == JsonValueKind.String
                ? modelProp.GetString() ?? ""
                : (root.TryGetProperty("type", out var typeProp) && typeProp.ValueKind == JsonValueKind.String
                    ? typeProp.GetString() ?? ""
                    : "");

            var appCode = root.TryGetProperty("app", out var appProp) && appProp.ValueKind == JsonValueKind.String
                ? appProp.GetString()
                : null;

            var deviceName = root.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String
                ? nameProp.GetString()
                : null;

            var mac = root.TryGetProperty("mac", out var macProp) && macProp.ValueKind == JsonValueKind.String
                ? FormatMacAddress(macProp.GetString())
                : null;

            var fw = root.TryGetProperty("fw_id", out var fwIdProp) && fwIdProp.ValueKind == JsonValueKind.String
                ? fwIdProp.GetString()
                : (root.TryGetProperty("fw", out var fwProp) && fwProp.ValueKind == JsonValueKind.String
                    ? fwProp.GetString()
                    : null);

            // Matcha mot våra starkt typade Shelly-produktklasser (först via intern model-kod, sedan via app-kod)
            var matched = catalog.FindMatch("Shelly", rawModel)
                          ?? catalog.FindMatch("Shelly", appCode);

            return new ShellyProbedInfo(
                IpAddress: cleanIp,
                DeviceName: deviceName,
                MacAddress: mac,
                RawModelCode: string.IsNullOrWhiteSpace(rawModel) ? (appCode ?? "Unknown") : rawModel,
                AppCode: appCode,
                Generation: matched?.Generation is > 0 ? matched.Generation : gen,
                FirmwareVersion: fw,
                MatchedProduct: matched);
        }
        catch
        {
            return null;
        }
    }

    private static string? FormatMacAddress(string? rawMac)
    {
        if (string.IsNullOrWhiteSpace(rawMac))
            return null;

        var hex = new string(rawMac.Where(Uri.IsHexDigit).ToArray()).ToUpperInvariant();
        if (hex.Length != 12)
            return rawMac;

        return string.Join(":", Enumerable.Range(0, 6).Select(i => hex.Substring(i * 2, 2)));
    }
}