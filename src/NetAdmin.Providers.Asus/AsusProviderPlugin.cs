using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Asus;

public sealed class AsusProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Asus";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register module and service explicitly when plugin loader runs
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, AsusRouterHardwareUiModule>());
        services.TryAddSingleton<AsusService>();
    }
}
