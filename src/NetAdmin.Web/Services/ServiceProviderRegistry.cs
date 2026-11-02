using Microsoft.Extensions.DependencyInjection;
using NetAdmin.Core.Models;
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

        var providers = AllProviders().ToList();
        var providerByName = providers.FirstOrDefault(provider =>
            string.Equals(provider.ProviderName, type, StringComparison.OrdinalIgnoreCase));
        if (providerByName is not null)
            return providerByName;

        return Enum.TryParse<ServiceType>(type, ignoreCase: true, out var serviceType)
            ? providers.FirstOrDefault(provider => provider.SupportedServiceTypes.Contains(serviceType))
            : null;
    }
}
