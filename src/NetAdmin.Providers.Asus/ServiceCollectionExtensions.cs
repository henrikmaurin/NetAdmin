using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Asus;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsusProvider(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, AsusRouterHardwareUiModule>());
        services.TryAddSingleton<AsusService>();
        return services;
    }
}
