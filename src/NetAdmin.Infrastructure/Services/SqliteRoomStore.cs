using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class SqliteRoomStore(IDbContextFactory<AppDbContext> dbFactory) : IRoomProvider
{
    public async Task<IReadOnlyList<RoomItem>> GetRoomsAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entities = await db.Rooms.OrderBy(r => r.BuildingOrZone).ThenBy(r => r.Name).ToListAsync(ct);
        return entities.Select(e => new RoomItem(e.Id, e.Name, e.BuildingOrZone, e.Floor, e.TagsCsv)).ToList();
    }

    public async Task SaveRoomAsync(RoomItem room, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.Rooms.FirstOrDefaultAsync(r => r.Id == room.Id, ct);

        if (entity is null)
        {
            db.Rooms.Add(new RoomEntity
            {
                Id = room.Id,
                Name = room.Name,
                BuildingOrZone = room.BuildingOrZone,
                Floor = room.Floor,
                TagsCsv = room.TagsCsv
            });
        }
        else
        {
            var oldName = entity.Name;
            entity.Name = room.Name;
            entity.BuildingOrZone = room.BuildingOrZone;
            entity.Floor = room.Floor;
            entity.TagsCsv = room.TagsCsv;

            if (!string.Equals(oldName, room.Name, StringComparison.Ordinal))
            {
                var hardwareInRoom = await db.HardwareDevices
                    .Where(h => h.Room == oldName)
                    .ToListAsync(ct);
                foreach (var hw in hardwareInRoom)
                {
                    hw.Room = room.Name;
                }

                var shellyInRoom = await db.ShellyDevices
                    .Where(s => s.Room == oldName)
                    .ToListAsync(ct);
                foreach (var s in shellyInRoom)
                {
                    s.Room = room.Name;
                }

                var patchPortsInRoom = await db.PatchPanelPorts
                    .Where(p => p.Room == oldName)
                    .ToListAsync(ct);
                foreach (var p in patchPortsInRoom)
                {
                    p.Room = room.Name;
                }

                var patchPortItemsInRoom = await db.PatchPanelPortItems
                    .Where(p => p.Room == oldName)
                    .ToListAsync(ct);
                foreach (var p in patchPortItemsInRoom)
                {
                    p.Room = room.Name;
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteRoomAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.Rooms.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (entity is not null)
        {
            db.Rooms.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}