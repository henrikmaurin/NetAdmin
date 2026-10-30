using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Frigate;

public sealed class FrigateProviderPlugin : NetAdminProviderPluginBase
{
    public override string ProviderName => "Frigate";

    protected override void RegisterProviderServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddFrigateProvider();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrigateProvider(this IServiceCollection services)
    {
        services.AddHttpClient<FrigateProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        services.AddScoped<IFrigateProvider>(sp => sp.GetRequiredService<FrigateProvider>());
        return services;
    }
}
