using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;
using NetAdmin.Providers.Frigate;
using NetAdmin.Providers.HomeAssistant;
using NetAdmin.Web.Services;
using Xunit;

namespace NetAdmin.Web.Tests;

public class ServiceProviderRegistryTests
{
    [Fact]
    public void GetProviderForType_ResolvesDeclaredServiceType()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IServiceIntegration>(new StubServiceIntegration("Frigate", ServiceType.Nvr));
        var homeAssistant = new StubServiceIntegration("Home Assistant", ServiceType.HomeAutomation);
        services.AddSingleton<IServiceIntegration>(homeAssistant);
        using var provider = services.BuildServiceProvider();
        var registry = new ServiceProviderRegistry(provider);

        var result = registry.GetProviderForType("HomeAutomation");

        Assert.Same(homeAssistant, result);
    }

    [Fact]
    public void ProviderRegistrations_AreVisibleToServiceRegistry()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddScoped<IEncryptedSettingsService, StubEncryptedSettingsService>();
        services.AddHomeAssistantProvider();
        services.AddFrigateProvider();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var registry = new ServiceProviderRegistry(scope.ServiceProvider);

        Assert.IsType<HomeAssistantProvider>(registry.GetProviderForType("HomeAutomation"));
        Assert.IsType<FrigateProvider>(registry.GetProviderForType("Nvr"));
    }

    [Fact]
    public void GetProviderForType_ReturnsNullForUnknownTypeInsteadOfFirstProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IServiceIntegration>(new StubServiceIntegration("Frigate", ServiceType.Nvr));
        using var provider = services.BuildServiceProvider();
        var registry = new ServiceProviderRegistry(provider);

        Assert.Null(registry.GetProviderForType("Unknown"));
    }

    private sealed class StubEncryptedSettingsService : IEncryptedSettingsService
    {
        public Task<string?> GetSettingAsync(string key, string? fallbackConfigPath = null, CancellationToken ct = default) =>
            Task.FromResult<string?>(null);

        public Task SaveSettingAsync(string key, string? plainTextValue, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class StubServiceIntegration(string name, ServiceType serviceType) : IServiceIntegration
    {
        public string ProviderName => name;
        public IReadOnlyCollection<ServiceType> SupportedServiceTypes => [serviceType];

        public Task<bool> TestConnectionAsync(CancellationToken ct = default) => Task.FromResult(true);

        public Task<ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, string? linkedDeviceId = null, CancellationToken ct = default) =>
            Task.FromResult<ServiceStatus?>(null);
    }
}
