using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using NetAdmin.Infrastructure.Data;
using NetAdmin.Core.Services;

namespace NetAdmin.Infrastructure.Services;

public class EncryptedSettingsService(
    IDbContextFactory<AppDbContext> dbFactory,
    IDataProtectionProvider protectionProvider,
    IConfiguration configuration)
    : IEncryptedSettingsService
{
    private readonly IDataProtector _protector = protectionProvider.CreateProtector("NetAdmin.Settings.v1");

    public async Task<string?> GetSettingAsync(string key, string? fallbackConfigPath = null, CancellationToken ct = default)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(ct);
            var entity = await db.Set<SystemSettingEntity>().FirstOrDefaultAsync(s => s.Key == key, ct);

            if (entity is not null && !string.IsNullOrWhiteSpace(entity.EncryptedValue))
            {
                return _protector.Unprotect(entity.EncryptedValue);
            }
        }
        catch
        {
            // Faller tillbaka till IConfiguration om databasen inte är tillgänglig
        }

        return !string.IsNullOrWhiteSpace(fallbackConfigPath)
            ? configuration[fallbackConfigPath]
            : configuration[key];
    }

    public async Task SaveSettingAsync(string key, string? plainTextValue, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.Set<SystemSettingEntity>().FirstOrDefaultAsync(s => s.Key == key, ct);

        var encrypted = string.IsNullOrWhiteSpace(plainTextValue)
            ? string.Empty
            : _protector.Protect(plainTextValue);

        if (entity is null)
        {
            db.Set<SystemSettingEntity>().Add(new SystemSettingEntity
            {
                Key = key,
                EncryptedValue = encrypted
            });
        }
        else
        {
            entity.EncryptedValue = encrypted;
        }

        await db.SaveChangesAsync(ct);
    }
}
