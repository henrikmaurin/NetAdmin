using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class SqlitePatchPanelStore(IDbContextFactory<AppDbContext> dbFactory) : IPatchPanelProvider
{
    public async Task<IReadOnlyList<PatchPanelPort>> GetPortsAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entities = await db.PatchPanelPorts.OrderBy(p => p.PortNumber).ToListAsync(ct);

        // Standard är nu 8 portar om tabellen är tom
        if (entities.Count == 0)
        {
            for (int i = 1; i <= 8; i++)
            {
                db.PatchPanelPorts.Add(new PatchPanelPortEntity
                {
                    PortNumber = i,
                    Label = $"Uttag {i}",
                    Room = "",
                    TagsCsv = "patchpanel"
                });
            }
            await db.SaveChangesAsync(ct);
            entities = await db.PatchPanelPorts.OrderBy(p => p.PortNumber).ToListAsync(ct);
        }

        return entities.Select(e => new PatchPanelPort(e.PortNumber, e.Label, e.Room, e.TagsCsv)).ToList();
    }

    public async Task SavePortAsync(PatchPanelPort port, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.PatchPanelPorts.FirstOrDefaultAsync(p => p.PortNumber == port.PortNumber, ct);

        if (entity is not null)
        {
            entity.Label = port.Label;
            entity.Room = port.Room;
            entity.TagsCsv = port.TagsCsv;
            await db.SaveChangesAsync(ct);
        }
    }

    public async Task SetPortCountAsync(int portCount, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var existing = await db.PatchPanelPorts.ToListAsync(ct);

        // Ta bort portar som överstiger valt antal (t.ex. när man ändrar från 24 till 8)
        var toRemove = existing.Where(p => p.PortNumber > portCount).ToList();
        if (toRemove.Count > 0)
        {
            db.PatchPanelPorts.RemoveRange(toRemove);
        }

        // Lägg till portar om antalet utökas
        for (int i = 1; i <= portCount; i++)
        {
            if (!existing.Any(p => p.PortNumber == i))
            {
                db.PatchPanelPorts.Add(new PatchPanelPortEntity
                {
                    PortNumber = i,
                    Label = $"Uttag {i}",
                    Room = "",
                    TagsCsv = "patchpanel"
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }
}