using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Ikea;

public class IkeaProvider : IIkeaProvider
{
    public string ProviderName => "IKEA Home Smart (TRÅDFRI / DIRIGERA)";

    public Task<bool> TestConnectionAsync(CancellationToken ct = default) => Task.FromResult(true);
}
