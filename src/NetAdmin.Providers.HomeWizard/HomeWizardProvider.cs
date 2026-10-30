using System.Text.Json;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.HomeWizard;

public class HomeWizardProvider(
    ProductCatalogRegistry catalog,
    HttpClient httpClient) : IHomeWizardProvider
{
    public string ProviderName => "HomeWizard Energy";

    public Task<bool> TestConnectionAsync(CancellationToken ct = default) => Task.FromResult(true);

    /// <summary>
    /// Identifierar en HomeWizard Energy-enhet på det lokala nätverket via http://{ip}/api
    /// (exempel: { "product_name": "P1 Meter", "product_type": "HWE-P1", "serial": "3c39e7...", "firmware_version": "4.19", "api_version": "v1" }).
    /// </summary>
    public async Task<HomeWizardProbedInfo?> ProbeDeviceAsync(string ipAddress, CancellationToken ct = default)
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
            var url = $"http://{cleanIp}/api";
            using var response = await httpClient.GetAsync(url, cts.Token);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cts.Token);
            var root = doc.RootElement;

            var productType = root.TryGetProperty("product_type", out var ptProp) && ptProp.ValueKind == JsonValueKind.String
                ? ptProp.GetString() ?? ""
                : "";

            var productName = root.TryGetProperty("product_name", out var pnProp) && pnProp.ValueKind == JsonValueKind.String
                ? pnProp.GetString() ?? "HomeWizard Energy"
                : "HomeWizard Energy";

            var serial = root.TryGetProperty("serial", out var serProp) && serProp.ValueKind == JsonValueKind.String
                ? FormatMacAddress(serProp.GetString())
                : null;

            var fw = root.TryGetProperty("firmware_version", out var fwProp) && fwProp.ValueKind == JsonValueKind.String
                ? fwProp.GetString()
                : null;

            var apiVer = root.TryGetProperty("api_version", out var apiProp) && apiProp.ValueKind == JsonValueKind.String
                ? apiProp.GetString()
                : null;

            var matched = catalog.FindMatch("HomeWizard", productType)
                          ?? catalog.FindMatch("HomeWizard", productName);

            return new HomeWizardProbedInfo(
                IpAddress: cleanIp,
                ProductName: productName,
                ProductTypeCode: productType,
                SerialHex: serial,
                FirmwareVersion: fw,
                ApiVersion: apiVer,
                MatchedProduct: matched);
        }
        catch
        {
            return null;
        }
    }

    private static string? FormatMacAddress(string? rawHex)
    {
        if (string.IsNullOrWhiteSpace(rawHex))
            return null;

        var hex = new string(rawHex.Where(Uri.IsHexDigit).ToArray()).ToUpperInvariant();
        if (hex.Length != 12)
            return rawHex;

        return string.Join(":", Enumerable.Range(0, 6).Select(i => hex.Substring(i * 2, 2)));
    }
}
