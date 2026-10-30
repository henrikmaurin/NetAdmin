using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NetAdmin.Core.Providers;

/// <summary>
/// Contract for a hardware/network provider plugin assembly.
/// Discovered and invoked automatically via reflection at startup.
/// Note: All <see cref="NetAdmin.Core.Models.HardwareProductDefinition"/> and <see cref="IHardwareUiModule"/>
/// classes in provider assemblies are also registered automatically by the plugin loader.
/// </summary>
public interface INetAdminProviderPlugin
{
    string ProviderName { get; }

    void RegisterServices(IServiceCollection services, IConfiguration configuration);
}
