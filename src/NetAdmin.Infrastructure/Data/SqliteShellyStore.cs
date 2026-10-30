using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Providers;

namespace NetAdmin.Infrastructure.Data;

public class SqliteShellyStore(IDbContextFactory<AppDbContext> dbFactory) : IShellyStore
{
    public async Task<IReadOnlyList<ShellyDevice>> GetDevicesAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var list = await db.ShellyDevices.OrderBy(s => s.Name).ToListAsync(ct);
        return list.Select(e => new ShellyDevice(e.Id, e.Name, e.IpAddress, e.Model, e.Room, e.TagsCsv)).ToList();
    }

    public async Task SaveDeviceAsync(ShellyDevice device, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.ShellyDevices.FirstOrDefaultAsync(s => s.Id == device.Id, ct);

        if (entity is null)
        {
            db.ShellyDevices.Add(new ShellyDeviceEntity
            {
                Id = device.Id,
                Name = device.Name,
                IpAddress = device.IpAddress,
                Model = device.Model,
                Room = device.Room,
                TagsCsv = device.TagsCsv
            });
        }
        else
        {
            entity.Name = device.Name;
            entity.IpAddress = device.IpAddress;
            entity.Model = device.Model;
            entity.Room = device.Room;
            entity.TagsCsv = device.TagsCsv;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteDeviceAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.ShellyDevices.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is not null)
        {
            db.ShellyDevices.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}