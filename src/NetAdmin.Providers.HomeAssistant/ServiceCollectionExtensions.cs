using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.HomeAssistant;

public sealed class HomeAssistantProviderPlugin : NetAdminProviderPluginBase
{
    public override string ProviderName => "HomeAssistant";

    protected override void RegisterProviderServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHomeAssistantProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHomeAssistantProvider(this IServiceCollection services)
    {
        services.AddHttpClient<HomeAssistantProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        services.AddScoped<IHomeAssistantProvider>(sp => sp.GetRequiredService<HomeAssistantProvider>());
        return services;
    }
}
