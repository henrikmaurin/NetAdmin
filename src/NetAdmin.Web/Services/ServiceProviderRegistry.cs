using Microsoft.Extensions.DependencyInjection;
using NetAdmin.Core.Providers;

namespace NetAdmin.Web.Services;

/// <summary>
/// Lightweight registry to resolve IServiceIntegration providers by name.
/// Resolves providers from the request's service provider so we don't capture scoped services in singletons.
/// </summary>
public class ServiceProviderRegistry
{
    private readonly IServiceProvider _services;

    public ServiceProviderRegistry(IServiceProvider services)
    {
        _services = services;
    }

    public IEnumerable<IServiceIntegration> AllProviders()
    {
        return _services.GetService<IEnumerable<IServiceIntegration>>() ?? Array.Empty<IServiceIntegration>();
    }

    public IServiceIntegration? GetProviderForType(string type)
    {
        if (string.IsNullOrWhiteSpace(type)) return null;
        var all = AllProviders();
        // Prefer case-insensitive exact name match
        var match = all.FirstOrDefault(p => string.Equals(p.ProviderName, type, StringComparison.OrdinalIgnoreCase));
        return match ?? all.FirstOrDefault();
    }
}
