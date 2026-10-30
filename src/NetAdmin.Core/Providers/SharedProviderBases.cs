using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Services;

namespace NetAdmin.Core.Providers;

/// <summary>
/// Shared base class for HTTP-based provider integrations.
/// Centralizes URL normalization, encrypted setting resolution, authentication headers, and JSON parsing.
/// </summary>
public abstract class HttpIntegrationProviderBase(
    HttpClient httpClient,
    IEncryptedSettingsService settingsService) : INetworkIntegration
{
    protected HttpClient HttpClient => httpClient;
    protected IEncryptedSettingsService SettingsService => settingsService;

    public abstract string ProviderName { get; }

    public abstract Task<bool> TestConnectionAsync(CancellationToken ct = default);

    protected static string NormalizeBaseUrl(string? rawUrlOrIp, string defaultScheme = "http", int? defaultPort = null)
    {
        if (string.IsNullOrWhiteSpace(rawUrlOrIp))
            return string.Empty;

        var trimmed = rawUrlOrIp.Trim().TrimEnd('/');
        if (!trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = $"{defaultScheme}://{trimmed}";
        }

        if (defaultPort.HasValue && Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) && uri.IsDefaultPort)
        {
            var builder = new UriBuilder(uri) { Port = defaultPort.Value };
            return builder.Uri.ToString().TrimEnd('/');
        }

        return trimmed;
    }

    protected async Task<string?> ResolveSettingAsync(
        string? deviceId,
        string deviceKeySuffix,
        string globalSettingKey,
        string? fallbackValue = null,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(deviceId))
        {
            var deviceVal = await SettingsService.GetSettingAsync($"device:{deviceId}:{deviceKeySuffix}", null, ct);
            if (!string.IsNullOrWhiteSpace(deviceVal))
                return deviceVal.Trim();
        }

        var globalVal = await SettingsService.GetSettingAsync(globalSettingKey, globalSettingKey, ct);
        if (!string.IsNullOrWhiteSpace(globalVal))
            return globalVal.Trim();

        return string.IsNullOrWhiteSpace(fallbackValue) ? null : fallbackValue.Trim();
    }

    protected static HttpRequestMessage CreateAuthenticatedGetRequest(string url, string? bearerToken = null, string? basicUser = null, string? basicPassword = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken.Trim());
        }
        else if (!string.IsNullOrWhiteSpace(basicUser) && !string.IsNullOrWhiteSpace(basicPassword))
        {
            var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{basicUser}:{basicPassword}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", raw);
        }

        return request;
    }

    protected async Task<JsonDocument?> SendAndParseJsonAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await HttpClient.SendAsync(request, cts.Token);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token);
            return await JsonDocument.ParseAsync(stream, cancellationToken: cts.Token);
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Shared base class for provider plugins that registers assembly-defined IHardwareUiModule and HardwareProductDefinition types.
/// </summary>
public abstract class NetAdminProviderPluginBase : INetAdminProviderPlugin
{
    public abstract string ProviderName { get; }

    public virtual void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        RegisterProviderServices(services, configuration);

        services.TryAddSingleton<ProductCatalogRegistry>();
        var assembly = GetType().Assembly;

        foreach (var type in assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false }))
        {
            if (typeof(HardwareProductDefinition).IsAssignableFrom(type))
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
            }

            if (typeof(IHardwareUiModule).IsAssignableFrom(type))
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHardwareUiModule), type));
            }
        }
    }

    protected abstract void RegisterProviderServices(IServiceCollection services, IConfiguration configuration);
}

/// <summary>
/// Shared base class for category-based IHardwareUiModule implementations to minimize boilerplate.
/// </summary>
public abstract class HardwareUiModuleBase : IHardwareUiModule
{
    public abstract string ModuleId { get; }
    public abstract string DisplayName { get; }
    public abstract string Icon { get; }
    public abstract HardwareCategory Category { get; }
    public virtual int Priority => 80;

    public virtual Type? EditorComponentType => null;
    public virtual Type? HeaderActionsComponentType => null;
    public virtual Type? CardSummaryComponentType => null;

    protected virtual IReadOnlyList<string> MatchKeywords => [];
    protected virtual string DefaultDeviceName => $"New {Category.ToDisplayName()}";
    protected virtual string DefaultRoom => "Server Closet";
    protected virtual string DefaultVendor => "Generic";
    protected virtual string DefaultModel => Category.ToDisplayName();
    protected virtual int DefaultPortCount => 1;
    protected virtual string DefaultTagsCsv => Category.ToString().ToLowerInvariant();

    public virtual bool CanHandle(HardwareDevice device) =>
        device.Category == Category ||
        (device.Category == HardwareCategory.Generic && MatchesKeywords(device.Vendor, device.TagsCsv, device.Name));

    public virtual bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == Category ||
        (category == HardwareCategory.Generic && MatchesKeywords(vendor, tagsCsv, hostname));

    protected bool MatchesKeywords(string? vendor, string? tagsCsv, string? hostname)
    {
        foreach (var kw in MatchKeywords)
        {
            if ((!string.IsNullOrWhiteSpace(vendor) && vendor.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(tagsCsv) && tagsCsv.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(hostname) && hostname.Contains(kw, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }

    public virtual HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? DefaultDeviceName : name,
            Category: Category,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? DefaultRoom : room,
            PortCount: DefaultPortCount,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: DefaultTagsCsv,
            Vendor: DefaultVendor,
            Model: DefaultModel);

    public virtual HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = Category,
            Vendor = string.IsNullOrWhiteSpace(current.Vendor) ? DefaultVendor : current.Vendor,
            Model = string.IsNullOrWhiteSpace(current.Model) ? DefaultModel : current.Model,
            PortCount = current.PortCount <= 0 ? DefaultPortCount : current.PortCount,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? DefaultTagsCsv : current.TagsCsv
        };

    public virtual IEnumerable<DeviceCapability> GetCapabilities(HardwareDevice device) => [];

    public virtual IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) => [];
}
