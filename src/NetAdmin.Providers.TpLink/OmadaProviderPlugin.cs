using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.TpLink;

public sealed class OmadaProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Omada";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, OmadaControllerHardwareUiModule>());
        services.TryAddSingleton<OmadaService>();
    }
}
