using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Models;

namespace NetAdmin.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SwitchPortEntity> SwitchPorts => Set<SwitchPortEntity>();
    public DbSet<SystemSettingEntity> SystemSettings => Set<SystemSettingEntity>();
    public DbSet<ShellyDeviceEntity> ShellyDevices => Set<ShellyDeviceEntity>();
    public DbSet<UnmanagedSwitchEntity> UnmanagedSwitches => Set<UnmanagedSwitchEntity>();
    public DbSet<PatchPanelPortEntity> PatchPanelPorts => Set<PatchPanelPortEntity>();
    public DbSet<PhysicalLinkEntity> PhysicalLinks => Set<PhysicalLinkEntity>();
    public DbSet<ServiceEntity> Services => Set<ServiceEntity>();

    // Tabeller för polymorf hårdvara & flera patchpaneler
    public DbSet<HardwareDeviceEntity> HardwareDevices => Set<HardwareDeviceEntity>();
    public DbSet<PatchPanelPortItemEntity> PatchPanelPortItems => Set<PatchPanelPortItemEntity>();
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SwitchPortEntity>().HasKey(p => new { p.DeviceId, p.PortNumber });
        modelBuilder.Entity<SystemSettingEntity>().HasKey(s => s.Key);
        modelBuilder.Entity<ShellyDeviceEntity>().HasKey(s => s.Id);
        modelBuilder.Entity<UnmanagedSwitchEntity>().HasKey(s => s.Id);
        modelBuilder.Entity<PatchPanelPortEntity>().HasKey(p => p.PortNumber);
        modelBuilder.Entity<PhysicalLinkEntity>().HasKey(l => l.Id);
        modelBuilder.Entity<RoomEntity>().HasKey(r => r.Id);

        modelBuilder.Entity<HardwareDeviceEntity>().HasKey(h => h.Id);
        modelBuilder.Entity<PatchPanelPortItemEntity>().HasKey(p => new { p.PatchPanelId, p.PortNumber });
        modelBuilder.Entity<ServiceEntity>().HasKey(s => s.Id);
    }
}

public abstract class TaggableEntityBase
{
    public string TagsCsv { get; set; } = string.Empty;
}

public class SwitchPortEntity : TaggableEntityBase
{
    public string DeviceId { get; set; } = "tplink-sg108pe-1";
    public int PortNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public SwitchPortRole Role { get; set; }
    public int NativeVlanId { get; set; } = 1;
    public string TaggedVlanIdsCsv { get; set; } = string.Empty;
    public bool PoeEnabled { get; set; }
}

public class SystemSettingEntity
{
    public string Key { get; set; } = string.Empty;
    public string EncryptedValue { get; set; } = string.Empty;
}

public class ShellyDeviceEntity : TaggableEntityBase
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}

public class UnmanagedSwitchEntity : TaggableEntityBase
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int PortCount { get; set; } = 8;
    public string Location { get; set; } = string.Empty;
}

public class PatchPanelPortEntity : TaggableEntityBase
{
    public int PortNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}

public class PhysicalLinkEntity : TaggableEntityBase
{
    public string Id { get; set; } = string.Empty;
    public string SourceDeviceId { get; set; } = string.Empty;
    public string SourceDeviceName { get; set; } = string.Empty;
    public int SourcePort { get; set; }
    public string TargetDeviceId { get; set; } = string.Empty;
    public string TargetDeviceName { get; set; } = string.Empty;
    public int TargetPort { get; set; }
    public string CableLabel { get; set; } = string.Empty;
}

public class HardwareDeviceEntity : TaggableEntityBase
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public HardwareCategory Category { get; set; }
    public string? IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public string Room { get; set; } = string.Empty;
    public int PortCount { get; set; } = 1;
    public string? ManagementUrl { get; set; }
    public string Notes { get; set; } = string.Empty;

    public string Vendor { get; set; } = string.Empty;
    public string SubModel { get; set; } = string.Empty;
    public int ShellyGeneration { get; set; } = 2;
    public bool IsManaged { get; set; }
    public string SupportedProtocolsCsv { get; set; } = string.Empty;
    public string MeterInterface { get; set; } = string.Empty;
    public string StreamsJson { get; set; } = string.Empty;
}

public class PatchPanelPortItemEntity : TaggableEntityBase
{
    public string PatchPanelId { get; set; } = string.Empty;
    public int PortNumber { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}

public class RoomEntity : TaggableEntityBase
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BuildingOrZone { get; set; } = "Huset"; // T.ex. "Huset" eller "Sommarstuga"
    public string Floor { get; set; } = "Entréplan";
}

public class ServiceEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LinkedDeviceId { get; set; }
}
