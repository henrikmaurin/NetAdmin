using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetAdmin.Infrastructure.Data;
using NetAdmin.Infrastructure.Services;
using Xunit;

namespace NetAdmin.Infrastructure.Tests;

internal class TestDbContextFactory2 : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    public TestDbContextFactory2(DbContextOptions<AppDbContext> options) => _options = options;
    public AppDbContext CreateDbContext() => new AppDbContext(_options);
    public Task<AppDbContext> CreateDbContextAsync(System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(new AppDbContext(_options));
}

public class EncryptedSettingsServiceTests
{
    private static IDbContextFactory<AppDbContext> CreateInMemoryFactory(string name)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new TestDbContextFactory2(opts);
    }

    [Fact]
    public async Task SaveSetting_WritesEncryptedValue_And_GettingReturnsPlain()
    {
        var dbFactory = CreateInMemoryFactory(Guid.NewGuid().ToString());
        var protector = DataProtectionProvider.Create("NetAdmin.Tests");
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();

        var sut = new EncryptedSettingsService(dbFactory, protector, config);

        await sut.SaveSettingAsync("Key1", "SecretValue");

        // Directly read from DB to verify an encrypted value exists
        await using var db = await dbFactory.CreateDbContextAsync();
        var entity = db.Set<SystemSettingEntity>().FirstOrDefault(s => s.Key == "Key1");
        Assert.NotNull(entity);
        Assert.False(string.IsNullOrWhiteSpace(entity!.EncryptedValue));
        Assert.NotEqual("SecretValue", entity.EncryptedValue);

        // And GetSettingAsync should return the original plaintext
        var got = await sut.GetSettingAsync("Key1");
        Assert.Equal("SecretValue", got);
    }

    [Fact]
    public async Task GetSetting_FallsBackToConfiguration_WhenNotInDatabase()
    {
        var dbFactory = CreateInMemoryFactory(Guid.NewGuid().ToString());
        var protector = DataProtectionProvider.Create("NetAdmin.Tests");
        var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("Fallback:Path", "value-from-config") }).Build();

        var sut = new EncryptedSettingsService(dbFactory, protector, config);

        var got = await sut.GetSettingAsync("NonExistent", "Fallback:Path");
        Assert.Equal("value-from-config", got);
    }
}
