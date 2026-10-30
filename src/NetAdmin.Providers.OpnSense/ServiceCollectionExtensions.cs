using System.Net.Http.Headers;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.OpnSense;

public sealed class OpnSenseProviderPlugin : INetAdminProviderPlugin
{
    public string ProviderName => "OPNsense";

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpnSenseProvider(configuration);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpnSenseProvider(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OpnSenseOptions>(
            configuration.GetSection(OpnSenseOptions.SectionName));

        services.AddHttpClient<OpnSenseProvider>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OpnSenseOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
            client.Timeout = TimeSpan.FromSeconds(10);

            if (!string.IsNullOrWhiteSpace(options.ApiKey) && !string.IsNullOrWhiteSpace(options.ApiSecret))
            {
                var credentials = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{options.ApiKey}:{options.ApiSecret}"));
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", credentials);
            }
        })
        .ConfigurePrimaryHttpMessageHandler(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OpnSenseOptions>>().Value;
            var handler = new HttpClientHandler();
            if (options.IgnoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            }
            return handler;
        });

        // Exponera samma provider under dess kapabilitets-gränssnitt
        services.AddScoped<IRoutingProvider>(sp => sp.GetRequiredService<OpnSenseProvider>());
        services.AddScoped<IDhcpProvider>(sp => sp.GetRequiredService<OpnSenseProvider>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, RouterHardwareUiModule>());
        services.TryAddSingleton<ProductCatalogRegistry>();

        foreach (var type in typeof(ServiceCollectionExtensions).Assembly.GetTypes()
                     .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(HardwareProductDefinition).IsAssignableFrom(t)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
        }

        return services;
    }
}