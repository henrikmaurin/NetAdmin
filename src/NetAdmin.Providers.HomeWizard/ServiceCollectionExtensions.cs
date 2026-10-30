using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.HomeWizard;

public sealed class HomeWizardProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "HomeWizard";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHomeWizardProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHomeWizardProvider(this IServiceCollection services)
    {
        services.AddHttpClient<HomeWizardProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(4);
        });
        services.AddScoped<IHomeWizardProvider>(sp => sp.GetRequiredService<HomeWizardProvider>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, EnergyMeterHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}
