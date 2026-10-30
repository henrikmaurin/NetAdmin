namespace NetAdmin.Core.Models;

public enum ServiceType
{
    Other = 0,
    HomeAutomation = 1,
    Nvr = 2,
    Monitoring = 3,
    Storage = 4
}

public static class ServiceTypeExtensions
{
    public static string ToDisplayName(this ServiceType t) => t switch
    {
        ServiceType.Other => "Other",
        ServiceType.HomeAutomation => "Home Automation",
        ServiceType.Nvr => "NVR",
        ServiceType.Monitoring => "Monitoring",
        ServiceType.Storage => "Storage",
        _ => t.ToString()
    };
}

/// <summary>
/// A generic service status returned by providers implementing IServiceIntegration.
/// Providers may also include a provider-specific typed object in Details for richer UI.
/// </summary>
public record ServiceStatus(
    string ProviderName,
    string? State = null,
    string? Version = null,
    string? Summary = null,
    object? Details = null);
