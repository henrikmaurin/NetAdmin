using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Ikea;

public sealed class IkeaProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "IKEA";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIkeaProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIkeaProvider(this IServiceCollection services)
    {
        services.AddScoped<IIkeaProvider, IkeaProvider>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, IoTHubHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}
