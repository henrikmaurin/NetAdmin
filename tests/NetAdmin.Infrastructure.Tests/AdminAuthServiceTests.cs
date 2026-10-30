using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetAdmin.Core.Services;
using NetAdmin.Infrastructure.Data;
using NetAdmin.Infrastructure.Services;
using Xunit;

namespace NetAdmin.Infrastructure.Tests;

internal class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    public TestDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public AppDbContext CreateDbContext() => new AppDbContext(_options);
    public Task<AppDbContext> CreateDbContextAsync(System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(new AppDbContext(_options));
}

public class AdminAuthServiceTests
{
    private static IDbContextFactory<AppDbContext> CreateInMemoryFactory(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TestDbContextFactory(options);
    }

    private static IDataProtectionProvider CreateProtectionProvider() => DataProtectionProvider.Create("NetAdmin.Tests");

    private static IConfiguration CreateConfiguration() => new ConfigurationBuilder().AddInMemoryCollection().Build();

    [Fact]
    public async Task ConfigureAndValidateCredentials_Works()
    {
        var dbFactory = CreateInMemoryFactory(Guid.NewGuid().ToString());
        var protector = CreateProtectionProvider();
        var config = CreateConfiguration();

        var settings = new EncryptedSettingsService(dbFactory, protector, config);
        var svc = new AdminAuthService(settings);

        await svc.ConfigureAdminAsync("admin", "P@ssw0rd");

        Assert.True(await svc.IsAdminConfiguredAsync());

        Assert.True(await svc.ValidateCredentialsAsync("admin", "P@ssw0rd"));
        Assert.False(await svc.ValidateCredentialsAsync("admin", "wrong"));
        Assert.False(await svc.ValidateCredentialsAsync("other", "P@ssw0rd"));
    }

    [Fact]
    public async Task OnboardingFlags_ArePersisted()
    {
        var dbFactory = CreateInMemoryFactory(Guid.NewGuid().ToString());
        var protector = CreateProtectionProvider();
        var config = CreateConfiguration();

        var settings = new EncryptedSettingsService(dbFactory, protector, config);
        var svc = new AdminAuthService(settings);

        await svc.ConfigureAdminAsync("admin", "P@ssw0rd");
        Assert.False(await svc.IsOnboardingCompletedAsync());

        await svc.SetOnboardingCompletedAsync(true);
        Assert.True(await svc.IsOnboardingCompletedAsync());
    }
}
