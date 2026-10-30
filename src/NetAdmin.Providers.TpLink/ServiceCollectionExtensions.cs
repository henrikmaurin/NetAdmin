using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.TpLink;

public sealed class TpLinkProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "TP-Link";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTpLinkProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTpLinkProvider(this IServiceCollection services)
    {
        services.AddScoped<ISwitchProvider, TpLinkSg108PeProvider>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, SwitchHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}