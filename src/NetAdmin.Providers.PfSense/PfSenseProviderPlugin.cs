using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.PfSense;

public sealed class PfSenseProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "pfSense";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, PfSenseRouterHardwareUiModule>());
        services.TryAddSingleton<PfSenseService>();
    }
}
