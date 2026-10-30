using System.Net;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.UniFi;

public sealed class UniFiProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "Ubiquiti UniFi";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddUniFiProvider(configuration);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUniFiProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<UniFiOptions>(
            configuration.GetSection(UniFiOptions.SectionName));

        services.AddHttpClient<UniFiProvider>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UniFiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
            client.Timeout = TimeSpan.FromSeconds(10);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                client.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
            }
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptions<UniFiOptions>>().Value;
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            if (options.IgnoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }
            return handler;
        });

        services.AddScoped<IWirelessProvider>(sp => sp.GetRequiredService<UniFiProvider>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, AccessPointHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}