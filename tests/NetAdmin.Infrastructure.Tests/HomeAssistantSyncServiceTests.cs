using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;
using NetAdmin.Infrastructure.Data;
using NetAdmin.Infrastructure.Services;
using Xunit;

namespace NetAdmin.Infrastructure.Tests;

public class HomeAssistantSyncServiceTests
{
    [Fact]
    public async Task SyncEntities_PrefersMacAddressOverConflictingIpAddress()
    {
        var macDevice = new HardwareDevice("mac-device", "MAC match", HardwareCategory.Generic, "192.168.1.20", "AA:BB:CC:DD:EE:FF", "", 1, null, "");
        var ipDevice = new HardwareDevice("ip-device", "IP match", HardwareCategory.Generic, "192.168.1.10", null, "", 1, null, "");
        var entity = new HomeAssistantEntity(
            "device_tracker.phone",
            "home",
            "Phone",
            null,
            null,
            "{}",
            ["192.168.1.10", "aa-bb-cc-dd-ee-ff", "not-a-network-identifier"]);
        var service = CreateService([entity], [macDevice, ipDevice]);

        var results = await service.SyncEntitiesAsync();

        var result = Assert.Single(results);
        Assert.Equal(macDevice.Id, result.LinkedDeviceId);
        Assert.Equal(macDevice.Name, result.LinkedDeviceName);
        Assert.Equal(2, result.CandidateIdentifiers.Count);
    }

    [Fact]
    public async Task SyncEntities_MatchesShellyUsingDeviceRegistryMacIdentifier()
    {
        var shelly = new ShellyHardwareDevice("shelly-1", "Shelly Plus 1PM", "192.168.1.30", "AA:BB:CC:DD:EE:FF", "Kitchen");
        var entity = new HomeAssistantEntity("sensor.shelly_power", "12.4", "Shelly power", null, "W", "{}", ["aabbccddeeff"]);
        var service = CreateService([entity], [shelly]);

        var result = Assert.Single(await service.SyncEntitiesAsync());

        Assert.Equal(shelly.Id, result.LinkedDeviceId);
        Assert.Equal("homeassistant:sensor.shelly_power", result.ServiceId);
    }

    [Fact]
    public async Task SyncEntities_CreatesHomeAssistantInstanceServiceAndParentsItsEntities()
    {
        var homeAssistant = new HardwareDevice("ha-host", "Home Assistant", HardwareCategory.HomeAutomation, "192.168.1.2", null, "Server Closet", 1, null, "", "Home Assistant", "Green");
        var shelly = new ShellyHardwareDevice("shelly-1", "Shelly Plus 1PM", "192.168.1.30", "AA:BB:CC:DD:EE:FF", "Kitchen");
        var entity = new HomeAssistantEntity("sensor.shelly_power", "12.4", "Shelly power", null, "W", "{}", ["192.168.9.99"]);
        var unmatchedEntity = new HomeAssistantEntity("sensor.temperature", "20", "Temperature", null, null, "{}", []);
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var seedDb = await factory.CreateDbContextAsync())
        {
            seedDb.Services.Add(new ServiceEntity
            {
                Id = "homeassistant:sensor.shelly_power",
                Name = "Legacy power entity",
                Type = "HomeAutomation",
                LinkedDeviceId = shelly.Id
            });
            await seedDb.SaveChangesAsync();
        }

        var service = new HomeAssistantSyncService(factory, new FakeHomeAssistantProvider([entity, unmatchedEntity]), new FakeHardwareInventoryProvider([homeAssistant, shelly]));
        var syncResults = await service.SyncEntitiesAsync(homeAssistant.Id);

        await using var db = await factory.CreateDbContextAsync();
        var instanceService = await db.Services.SingleAsync(item => item.Id == "homeassistant:instance:ha-host");
        var entityService = await db.Services.SingleAsync(item => item.Id == "homeassistant:entity:ha-host:sensor.shelly_power");
        var unmatchedEntityService = await db.Services.SingleAsync(item => item.Id == "homeassistant:entity:ha-host:sensor.temperature");
        Assert.Equal(2, syncResults.Count);
        Assert.Equal(homeAssistant.Id, instanceService.LinkedDeviceId);
        Assert.True(instanceService.IsServiceInstance);
        Assert.Equal("homeassistant:instance:ha-host", entityService.ParentServiceId);
        Assert.Equal(shelly.Id, entityService.LinkedDeviceId);
        Assert.Equal("homeassistant:instance:ha-host", unmatchedEntityService.ParentServiceId);
        Assert.Null(unmatchedEntityService.LinkedDeviceId);
        Assert.Null(await db.Services.SingleOrDefaultAsync(item => item.Id == "homeassistant:sensor.shelly_power"));
    }

    [Fact]
    public async Task SyncEntities_AdoptsExistingHomeAssistantServiceInsteadOfCreatingDuplicate()
    {
        var homeAssistant = new HardwareDevice("ha-host", "Home Assistant", HardwareCategory.HomeAutomation, "192.168.1.2", null, "Server Closet", 1, null, "", "Home Assistant", "Green");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var seedDb = await factory.CreateDbContextAsync())
        {
            seedDb.Services.Add(new ServiceEntity
            {
                Id = "legacy-ha-service",
                Name = "Home Assistant",
                Type = ServiceType.HomeAutomation.ToString(),
                LinkedDeviceId = homeAssistant.Id
            });
            await seedDb.SaveChangesAsync();
        }

        var entity = new HomeAssistantEntity("sensor.temperature", "20", "Temperature", null, null, "{}", []);
        var service = new HomeAssistantSyncService(factory, new FakeHomeAssistantProvider([entity]), new FakeHardwareInventoryProvider([homeAssistant]));
        await service.SyncEntitiesAsync(homeAssistant.Id);

        await using var db = await factory.CreateDbContextAsync();
        var instanceService = await db.Services.SingleAsync(item => item.Id == "legacy-ha-service");
        var entityService = await db.Services.SingleAsync(item => item.ParentServiceId == instanceService.Id);
        Assert.True(instanceService.IsServiceInstance);
        Assert.Equal(homeAssistant.Id, instanceService.LinkedDeviceId);
        Assert.Null(entityService.LinkedDeviceId);
        Assert.Equal(2, await db.Services.CountAsync());
    }

    [Fact]
    public async Task SyncServiceEntities_PreservesDeviceScopedEntityIdsForLinkedHomeAssistantService()
    {
        var homeAssistant = new HardwareDevice("ha-host", "Home Assistant", HardwareCategory.HomeAutomation, "192.168.1.2", null, "Server Closet", 1, null, "");
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var seedDb = await factory.CreateDbContextAsync())
        {
            seedDb.Services.AddRange(
                new ServiceEntity { Id = "homeassistant:instance:ha-host", Name = "Home Assistant", Type = "HomeAutomation", LinkedDeviceId = homeAssistant.Id, IsServiceInstance = true },
                new ServiceEntity { Id = "homeassistant:entity:ha-host:sensor.temperature", Name = "Temperature", Type = "HomeAutomation", ParentServiceId = "homeassistant:instance:ha-host" });
            await seedDb.SaveChangesAsync();
        }

        var entity = new HomeAssistantEntity("sensor.temperature", "20", "Temperature", null, null, "{}", []);
        var service = new HomeAssistantSyncService(factory, new FakeHomeAssistantProvider([entity]), new FakeHardwareInventoryProvider([homeAssistant]));
        await service.SyncServiceEntitiesAsync("homeassistant:instance:ha-host");

        await using var db = await factory.CreateDbContextAsync();
        Assert.Equal(2, await db.Services.CountAsync());
        Assert.Equal("homeassistant:instance:ha-host", (await db.Services.SingleAsync(item => item.Id == "homeassistant:entity:ha-host:sensor.temperature")).ParentServiceId);
    }

    [Fact]
    public async Task SyncServiceEntities_PersistsEntitiesUnderHomeAssistantServiceWithoutHardwareDevice()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        await using (var seedDb = await factory.CreateDbContextAsync())
        {
            seedDb.Services.Add(new ServiceEntity
            {
                Id = "ha-service",
                Name = "Home Assistant",
                Type = ServiceType.HomeAutomation.ToString()
            });
            await seedDb.SaveChangesAsync();
        }

        var entity = new HomeAssistantEntity("sensor.temperature", "20", "Temperature", null, null, "{}", []);
        var service = new HomeAssistantSyncService(factory, new FakeHomeAssistantProvider([entity]), new FakeHardwareInventoryProvider([]));
        var result = Assert.Single(await service.SyncServiceEntitiesAsync("ha-service"));

        await using var db = await factory.CreateDbContextAsync();
        var instanceService = await db.Services.SingleAsync(item => item.Id == "ha-service");
        var entityService = await db.Services.SingleAsync(item => item.ParentServiceId == "ha-service");
        Assert.True(instanceService.IsServiceInstance);
        Assert.Equal("homeassistant:entity:ha-service:sensor.temperature", result.ServiceId);
        Assert.Null(instanceService.LinkedDeviceId);
        Assert.Null(entityService.LinkedDeviceId);
    }

    [Fact]
    public async Task SyncEntities_PersistsEntityWithoutNetworkAddressForManualLinking()
    {
        var entity = new HomeAssistantEntity("sensor.temperature", "20", "Temperature", null, null, "{}", []);
        var service = CreateService([entity], []);

        var result = Assert.Single(await service.SyncEntitiesAsync());

        Assert.Equal("homeassistant:sensor.temperature", result.ServiceId);
        Assert.Null(result.LinkedDeviceId);
        Assert.Empty(result.CandidateIdentifiers);
    }

    [Fact]
    public async Task SyncEntities_FallsBackToUniqueIpAddressMatch()
    {
        var device = new HardwareDevice("device-1", "IP match", HardwareCategory.Generic, "2001:db8::1", null, "", 1, null, "");
        var entity = new HomeAssistantEntity("device_tracker.phone", "home", "Phone", null, null, "{}", ["2001:0db8:0:0:0:0:0:1"]);
        var service = CreateService([entity], [device]);

        var results = await service.SyncEntitiesAsync();

        Assert.Equal(device.Id, Assert.Single(results).LinkedDeviceId);
    }

    [Fact]
    public async Task SyncEntities_LeavesAmbiguousIpUnlinkedAndPreservesManualLink()
    {
        var firstDevice = new HardwareDevice("device-1", "Device 1", HardwareCategory.Generic, "192.168.1.10", null, "", 1, null, "");
        var secondDevice = new HardwareDevice("device-2", "Device 2", HardwareCategory.Generic, "192.168.1.10", null, "", 1, null, "");
        var entity = new HomeAssistantEntity("device_tracker.phone", "home", "Phone", null, null, "{}", ["192.168.1.10"]);
        var service = CreateService([entity], [firstDevice, secondDevice]);

        var firstSync = await service.SyncEntitiesAsync();
        var discovered = Assert.Single(firstSync);
        Assert.Null(discovered.LinkedDeviceId);

        await service.SetServiceLinkAsync(discovered.ServiceId, secondDevice.Id);
        var secondSync = await service.SyncEntitiesAsync();

        var result = Assert.Single(secondSync);
        Assert.Equal(secondDevice.Id, result.LinkedDeviceId);
        Assert.Equal(secondDevice.Name, result.LinkedDeviceName);
    }

    private static HomeAssistantSyncService CreateService(
        IReadOnlyList<HomeAssistantEntity> entities,
        IReadOnlyList<HardwareDevice> hardware)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        IDbContextFactory<AppDbContext> factory = new TestContextFactory(options);
        return new HomeAssistantSyncService(factory, new FakeHomeAssistantProvider(entities), new FakeHardwareInventoryProvider(hardware));
    }

    private sealed class TestContextFactory(DbContextOptions<AppDbContext> options) : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);

        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new AppDbContext(options));
    }

    private sealed class FakeHomeAssistantProvider(IReadOnlyList<HomeAssistantEntity> entities) : IHomeAssistantProvider
    {
        public string ProviderName => "Home Assistant";

        public Task<bool> TestConnectionAsync(CancellationToken ct = default) => Task.FromResult(true);

        public Task<HomeAssistantStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default) =>
            Task.FromResult<HomeAssistantStatusSummary?>(null);

        public Task<IReadOnlyList<HomeAssistantEntity>> GetEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default) =>
            Task.FromResult(entities);

        public Task<IReadOnlyList<HomeAssistantEntity>> GetEntitiesForServiceAsync(string serviceId, string? linkedDeviceId = null, CancellationToken ct = default) =>
            Task.FromResult(entities);

        public Task<IReadOnlyList<HomeAssistantDevice>> GetDevicesForServiceAsync(string serviceId, string? linkedDeviceId = null, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<HomeAssistantDevice>>([]);
    }

    private sealed class FakeHardwareInventoryProvider(IReadOnlyList<HardwareDevice> hardware) : IHardwareInventoryProvider
    {
        public Task<IReadOnlyList<HardwareDevice>> GetAllHardwareAsync(CancellationToken ct = default) => Task.FromResult(hardware);

        public Task<HardwareDevice?> GetHardwareByIdAsync(string id, CancellationToken ct = default) =>
            Task.FromResult(hardware.FirstOrDefault(device => device.Id == id));

        public Task<HardwareDevice?> GetHardwareByMacAsync(string macAddress, CancellationToken ct = default) =>
            Task.FromResult(hardware.FirstOrDefault(device => MacAddress.Normalize(device.MacAddress) == MacAddress.Normalize(macAddress)));

        public Task SaveHardwareAsync(HardwareDevice device, CancellationToken ct = default) => Task.CompletedTask;

        public Task DeleteHardwareAsync(string id, CancellationToken ct = default) => Task.CompletedTask;

        public Task<IReadOnlyList<PatchPanelPortItem>> GetPatchPanelPortsAsync(string patchPanelId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<PatchPanelPortItem>>([]);

        public Task SavePatchPanelPortAsync(PatchPanelPortItem port, CancellationToken ct = default) => Task.CompletedTask;
    }
}
