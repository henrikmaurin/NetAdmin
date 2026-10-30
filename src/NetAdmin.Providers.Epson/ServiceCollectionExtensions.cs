using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Epson;

public sealed class EpsonProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Epson";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEpsonProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEpsonProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}
