using System.Threading;
using System.Threading.Tasks;

namespace NetAdmin.Core.Services;

public interface IEncryptedSettingsService
{
    Task<string?> GetSettingAsync(string key, string? fallbackConfigPath = null, CancellationToken ct = default);
    Task SaveSettingAsync(string key, string? plainTextValue, CancellationToken ct = default);
}
