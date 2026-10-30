using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Shelly;

public sealed class ShellyProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Shelly";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddShellyProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShellyProvider(this IServiceCollection services)
    {
        services.AddHttpClient<ShellyProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(4);
        });
        services.AddScoped<IShellyProvider>(sp => sp.GetRequiredService<ShellyProvider>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, ShellyHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}