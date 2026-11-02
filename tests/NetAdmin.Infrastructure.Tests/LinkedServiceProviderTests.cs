using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;
using NetAdmin.Infrastructure.Services;
using Xunit;

namespace NetAdmin.Infrastructure.Tests;

public class LinkedServiceProviderTests
{
    [Fact]
    public async Task Migration_AddsParentServiceOwnershipColumn()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var db = new AppDbContext(options))
        {
            await db.Database.MigrateAsync();
            db.Services.Add(new ServiceEntity
            {
                Id = "homeassistant:sensor.test",
                Name = "Test entity",
                Type = "HomeAutomation",
                ParentServiceId = "homeassistant:instance:ha-host"
            });
            await db.SaveChangesAsync();
        }

        await using var verifyDb = new AppDbContext(options);
        var service = await verifyDb.Services.SingleAsync();
        Assert.Equal("homeassistant:instance:ha-host", service.ParentServiceId);
    }

    [Fact]
    public async Task GetAvailableHomeAssistantServicesAsync_OnlyReturnsUnlinkedHomeAssistantServices()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var db = await factory.CreateDbContextAsync())
        {
            db.Services.AddRange(
                new ServiceEntity { Id = "homeassistant:instance:ha-host", Name = "Home Assistant", Type = "HomeAutomation", LinkedDeviceId = "ha-host", IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:instance:ha-host-2", Name = "Second Home Assistant", Type = "HomeAutomation", LinkedDeviceId = "ha-host-2", IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:sensor.shelly_power", Name = "Power", Type = "HomeAutomation", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:switch.shelly_relay", Name = "Relay", Type = "HomeAutomation", LinkedDeviceId = "other", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:light.shelly_desk", Name = "Desk", Type = "HomeAutomation", LinkedDeviceId = "shelly-1", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "homeassistant:entity:ha-host-2:sensor.other", Name = "Other instance entity", Type = "HomeAutomation", ParentServiceId = "homeassistant:instance:ha-host-2" },
                new ServiceEntity { Id = "frigate:camera.front_door", Name = "Front door", Type = "Nvr" });
            await db.SaveChangesAsync();
        }

        var provider = new SqliteLinkedServiceProvider(factory);
        var availableServices = await provider.GetAvailableHomeAssistantServicesAsync("ha-host");

        var service = Assert.Single(availableServices);
        Assert.Equal("homeassistant:entity:ha-host:sensor.shelly_power", service.Id);
    }

    [Fact]
    public async Task GetLinkedServicesForDevicesAsync_ReturnsEveryLinkedServiceGroupedByDevice()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var db = await factory.CreateDbContextAsync())
        {
            db.Services.AddRange(
                new ServiceEntity { Id = "homeassistant:instance:ha-host", Name = "Home Assistant", Type = "HomeAutomation", LinkedDeviceId = "ha-host", IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:sensor.power", Name = "Power", Type = "HomeAutomation", LinkedDeviceId = "camera-1", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "frigate:camera.front_door", Name = "Front door", Type = "Nvr", LinkedDeviceId = "camera-1" },
                new ServiceEntity { Id = "unrelated", Name = "Unrelated", Type = "Other", LinkedDeviceId = "other" });
            await db.SaveChangesAsync();
        }

        var provider = new SqliteLinkedServiceProvider(factory);
        var services = await provider.GetLinkedServicesForDevicesAsync(["camera-1", "ha-host"]);

        Assert.Collection(services["camera-1"],
            service => Assert.Equal("frigate:camera.front_door", service.Id),
            service => Assert.Equal("homeassistant:entity:ha-host:sensor.power", service.Id));
        Assert.Equal("homeassistant:instance:ha-host", Assert.Single(services["ha-host"]).Id);
        Assert.DoesNotContain("other", services.Keys);
    }

    [Fact]
    public async Task GetHomeAssistantEntitiesAsync_ResolvesServiceOnlyInstanceByServiceId()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var db = await factory.CreateDbContextAsync())
        {
            db.Services.AddRange(
                new ServiceEntity { Id = "ha-service", Name = "Home Assistant", Type = "HomeAutomation", IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:entity:ha-service:sensor.temperature", Name = "Temperature", Type = "HomeAutomation", ParentServiceId = "ha-service" });
            await db.SaveChangesAsync();
        }

        var provider = new SqliteLinkedServiceProvider(factory);
        var entity = Assert.Single(await provider.GetHomeAssistantEntitiesAsync("ha-service"));

        Assert.Equal("Temperature", entity.Name);
    }

    [Fact]
    public async Task GetLinkedServicesAsync_ReturnsServicesForTheRequestedDeviceInNameOrder()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var db = await factory.CreateDbContextAsync())
        {
            db.Services.AddRange(
                new ServiceEntity { Id = "homeassistant:instance:ha-host", Name = "Home Assistant", Type = "HomeAutomation", LinkedDeviceId = "ha-host", IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:sensor.shelly_power", Name = "Power", Type = "HomeAutomation", LinkedDeviceId = "shelly-1", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:switch.shelly_relay", Name = "Relay", Type = "HomeAutomation", LinkedDeviceId = "shelly-1", ParentServiceId = "homeassistant:instance:ha-host" },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:sensor.other", Name = "Other device", Type = "HomeAutomation", LinkedDeviceId = "other", ParentServiceId = "homeassistant:instance:ha-host" });
            await db.SaveChangesAsync();
        }

        var provider = new SqliteLinkedServiceProvider(factory);
        var linkedServices = await provider.GetLinkedServicesAsync("shelly-1");

        Assert.Collection(linkedServices,
            service => Assert.Equal(("Power", "homeassistant:entity:ha-host:sensor.shelly_power"), (service.Name, service.Id)),
            service => Assert.Equal(("Relay", "homeassistant:entity:ha-host:switch.shelly_relay"), (service.Name, service.Id)));

        var homeAssistantEntities = await provider.GetHomeAssistantEntitiesAsync("ha-host");
        Assert.Equal(3, homeAssistantEntities.Count);
        Assert.Contains(homeAssistantEntities, service => service.LinkedDeviceId == "shelly-1");

        var instanceService = Assert.Single(await provider.GetLinkedServicesAsync("ha-host"));
        Assert.True(instanceService.IsServiceInstance);
        Assert.Null(instanceService.ParentServiceDeviceId);
    }

    private sealed class TestContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new AppDbContext(options));
    }
}
