using System.Text.Json;
using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Models;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class SqliteHardwareInventoryStore(IDbContextFactory<AppDbContext> dbFactory) : IHardwareInventoryProvider
{
    public async Task<IReadOnlyList<HardwareDevice>> GetAllHardwareAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entities = await db.HardwareDevices.OrderBy(h => h.Category).ThenBy(h => h.Name).ToListAsync(ct);
        var patchPorts = await db.PatchPanelPortItems.OrderBy(p => p.PortNumber).ToListAsync(ct);

        return entities.Select(e => MapToDomain(e, patchPorts)).ToList();
    }

    public async Task<HardwareDevice?> GetHardwareByIdAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.HardwareDevices.FirstOrDefaultAsync(h => h.Id == id, ct);
        if (entity is null) return null;

        var ports = entity.Category == HardwareCategory.PatchPanel
            ? await db.PatchPanelPortItems.Where(p => p.PatchPanelId == id).OrderBy(p => p.PortNumber).ToListAsync(ct)
            : [];

        return MapToDomain(entity, ports);
    }

    public async Task<HardwareDevice?> GetHardwareByMacAsync(string macAddress, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(macAddress)) return null;
        var norm = MacAddress.Normalize(macAddress);

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.HardwareDevices.FirstOrDefaultAsync(h => h.MacAddress == norm, ct);
        if (entity is null) return null;

        return MapToDomain(entity, []);
    }

    public async Task SaveHardwareAsync(HardwareDevice device, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.HardwareDevices.FirstOrDefaultAsync(h => h.Id == device.Id, ct);

        if (entity is null)
        {
            entity = new HardwareDeviceEntity { Id = device.Id };
            db.HardwareDevices.Add(entity);
        }

        entity.Name = device.Name;
        entity.Category = device.Category;
        entity.IpAddress = device.IpAddress;
        entity.MacAddress = string.IsNullOrWhiteSpace(device.MacAddress) ? null : MacAddress.Normalize(device.MacAddress);
        entity.Room = device.Room;
        entity.PortCount = Math.Max(0, device.PortCount);
        entity.ManagementUrl = device.ManagementUrl;
        entity.Notes = device.Notes;
        entity.TagsCsv = device.TagsCsv;
        entity.Vendor = device.Vendor;
        entity.SubModel = device.Model;

        switch (device)
        {
            case ShellyHardwareDevice shelly:
                entity.Vendor = "Shelly";
                entity.SubModel = shelly.ShellyModel;
                entity.ShellyGeneration = shelly.Generation;
                break;
            case SwitchHardwareDevice sw:
                entity.IsManaged = sw.IsManaged;
                break;
            case IoTHubHardwareDevice hub:
                entity.SubModel = hub.HubModel;
                entity.SupportedProtocolsCsv = hub.SupportedProtocolsCsv;
                break;
            case EnergyMeterHardwareDevice meter:
                entity.SubModel = meter.MeterModel;
                entity.MeterInterface = meter.MeterInterface;
                break;
            case CameraHardwareDevice cam:
                entity.StreamsJson = JsonSerializer.Serialize(cam.EffectiveStreams);
                entity.NvrId = cam.NvrId;
                break;
        }

        await db.SaveChangesAsync(ct);

        // Om det är en Patchpanel: synkronisera antalet portar i PatchPanelPortItems
        if (device.Category == HardwareCategory.PatchPanel)
        {
            await SyncPatchPanelPortCountAsync(db, device.Id, entity.PortCount, ct);
        }
    }

    public async Task DeleteHardwareAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.HardwareDevices.FirstOrDefaultAsync(h => h.Id == id, ct);
        if (entity is not null)
        {
            var ports = await db.PatchPanelPortItems.Where(p => p.PatchPanelId == id).ToListAsync(ct);
            if (ports.Count > 0) db.PatchPanelPortItems.RemoveRange(ports);

            db.HardwareDevices.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<PatchPanelPortItem>> GetPatchPanelPortsAsync(string patchPanelId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var ports = await db.PatchPanelPortItems
            .Where(p => p.PatchPanelId == patchPanelId)
            .OrderBy(p => p.PortNumber)
            .ToListAsync(ct);

        return ports.Select(p => new PatchPanelPortItem(p.PatchPanelId, p.PortNumber, p.Label, p.Room, p.TagsCsv)).ToList();
    }

    public async Task SavePatchPanelPortAsync(PatchPanelPortItem port, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.PatchPanelPortItems
            .FirstOrDefaultAsync(p => p.PatchPanelId == port.PatchPanelId && p.PortNumber == port.PortNumber, ct);

        if (entity is not null)
        {
            entity.Label = port.Label;
            entity.Room = port.Room;
            entity.TagsCsv = port.TagsCsv;
            await db.SaveChangesAsync(ct);
        }
    }

    private static async Task SyncPatchPanelPortCountAsync(AppDbContext db, string panelId, int targetCount, CancellationToken ct)
    {
        var existing = await db.PatchPanelPortItems.Where(p => p.PatchPanelId == panelId).ToListAsync(ct);

        var toRemove = existing.Where(p => p.PortNumber > targetCount).ToList();
        if (toRemove.Count > 0)
        {
            db.PatchPanelPortItems.RemoveRange(toRemove);
        }

        for (int i = 1; i <= targetCount; i++)
        {
            if (!existing.Any(p => p.PortNumber == i))
            {
                db.PatchPanelPortItems.Add(new PatchPanelPortItemEntity
                {
                    PatchPanelId = panelId,
                    PortNumber = i,
                    Label = $"Uttag {i}",
                    Room = "",
                    TagsCsv = "uttag"
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static HardwareDevice MapToDomain(HardwareDeviceEntity e, List<PatchPanelPortItemEntity> allPatchPorts)
    {
        bool isShelly = e.Vendor.Equals("Shelly", StringComparison.OrdinalIgnoreCase) ||
                        e.TagsCsv.Contains("shelly", StringComparison.OrdinalIgnoreCase);

        if (e.Category == HardwareCategory.IoT && isShelly)
        {
            return new ShellyHardwareDevice(
                e.Id, e.Name, e.IpAddress, e.MacAddress, e.Room,
                string.IsNullOrWhiteSpace(e.SubModel) ? "Shelly" : e.SubModel,
                e.ShellyGeneration, e.Notes, e.TagsCsv);
        }

        return e.Category switch
        {
            HardwareCategory.Camera => new CameraHardwareDevice(
                e.Id, e.Name, e.IpAddress, e.MacAddress, e.Room,
                e.Vendor, e.SubModel, e.PortCount,
                DeserializeStreams(e.StreamsJson).FirstOrDefault()?.Url,
                e.Notes, e.NvrId, e.TagsCsv,
                DeserializeStreams(e.StreamsJson)),

            HardwareCategory.Switch => new SwitchHardwareDevice(
                e.Id, e.Name, e.IpAddress, e.MacAddress, e.Room,
                e.PortCount, e.IsManaged, e.Vendor, e.SubModel, e.ManagementUrl, e.Notes, e.TagsCsv),

            HardwareCategory.PatchPanel => new PatchPanelHardwareDevice(
                e.Id, e.Name, e.Room, e.PortCount, e.Notes, e.TagsCsv,
                allPatchPorts.Where(p => p.PatchPanelId == e.Id)
                    .OrderBy(p => p.PortNumber)
                    .Select(p => new PatchPanelPortItem(p.PatchPanelId, p.PortNumber, p.Label, p.Room, p.TagsCsv))
                    .ToList()),

            HardwareCategory.FiberConverter => new FiberConverterHardwareDevice(
                e.Id, e.Name, e.Room, e.PortCount,
                e.Vendor, string.IsNullOrWhiteSpace(e.SubModel) ? "Mediaomvandlare / ONT" : e.SubModel,
                e.Notes, e.TagsCsv),

            HardwareCategory.WanEndpoint => new WanHardwareDevice(
                e.Id, e.Name, e.IpAddress,
                string.IsNullOrWhiteSpace(e.Vendor) ? e.SubModel : e.Vendor,
                e.Notes, e.TagsCsv),

            HardwareCategory.IoTHub => new IoTHubHardwareDevice(
                e.Id, e.Name, e.IpAddress, e.MacAddress, e.Room,
                string.IsNullOrWhiteSpace(e.Vendor) ? "IKEA" : e.Vendor,
                string.IsNullOrWhiteSpace(e.SubModel) ? "IoT Hub" : e.SubModel,
                e.PortCount,
                string.IsNullOrWhiteSpace(e.SupportedProtocolsCsv) ? "Zigbee, Ethernet" : e.SupportedProtocolsCsv,
                e.ManagementUrl, e.Notes, e.TagsCsv),

            HardwareCategory.EnergyMeter => new EnergyMeterHardwareDevice(
                e.Id, e.Name, e.IpAddress, e.MacAddress, e.Room,
                string.IsNullOrWhiteSpace(e.Vendor) ? "HomeWizard" : e.Vendor,
                string.IsNullOrWhiteSpace(e.SubModel) ? "Energimätare" : e.SubModel,
                e.PortCount,
                string.IsNullOrWhiteSpace(e.MeterInterface) ? "RJ12 P1 / HAN" : e.MeterInterface,
                e.ManagementUrl, e.Notes, e.TagsCsv),

            _ => new HardwareDevice(
                e.Id, e.Name, e.Category, e.IpAddress, e.MacAddress, e.Room,
                e.PortCount, e.ManagementUrl, e.Notes, e.TagsCsv, e.Vendor, e.SubModel)
        };
    }

    private static IReadOnlyList<CameraStreamDefinition> DeserializeStreams(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<CameraStreamDefinition>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
