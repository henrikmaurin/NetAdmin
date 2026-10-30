using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.OpenWrt;

public sealed class OpenWrtProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "OpenWrt";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, OpenWrtRouterHardwareUiModule>());
        services.TryAddSingleton<OpenWrtService>();
    }
}
