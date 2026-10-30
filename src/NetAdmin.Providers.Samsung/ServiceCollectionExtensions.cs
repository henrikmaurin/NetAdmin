using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Samsung;

public sealed class SamsungProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Samsung";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSamsungProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSamsungProvider(this IServiceCollection services)
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
