using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetAdmin.Infrastructure.Data;
using Xunit;

namespace NetAdmin.Infrastructure.Tests;

internal class TestDbContextFactory3 : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;
    public TestDbContextFactory3(DbContextOptions<AppDbContext> options) => _options = options;
    public AppDbContext CreateDbContext() => new AppDbContext(_options);
    public Task<AppDbContext> CreateDbContextAsync(System.Threading.CancellationToken cancellationToken = default) => Task.FromResult(new AppDbContext(_options));
}

public class ServiceEntityTests
{
    private static IDbContextFactory<AppDbContext> CreateInMemoryFactory(string name)
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        return new TestDbContextFactory3(opts);
    }

    [Fact]
    public async Task CanCreateReadUpdateDelete_ServiceEntity()
    {
        var dbFactory = CreateInMemoryFactory(Guid.NewGuid().ToString());
        await using var db = await dbFactory.CreateDbContextAsync();

        var s = new ServiceEntity
        {
            Id = "svc1",
            Name = "Home Assistant",
            Type = "HomeAutomation",
            Description = "HA instance",
            LinkedDeviceId = null
        };

        db.Services.Add(s);
        await db.SaveChangesAsync();

        var read = await db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == "svc1");
        Assert.NotNull(read);
        Assert.Equal("Home Assistant", read.Name);

        // Update
        read.Description = "Updated";
        db.Services.Update(read);
        await db.SaveChangesAsync();

        var updated = await db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == "svc1");
        Assert.Equal("Updated", updated.Description);

        db.Services.Remove(updated);
        await db.SaveChangesAsync();

        var gone = await db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == "svc1");
        Assert.Null(gone);
    }
}
