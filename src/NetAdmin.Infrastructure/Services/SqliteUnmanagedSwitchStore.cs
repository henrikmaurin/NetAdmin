using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class SqliteUnmanagedSwitchStore(IDbContextFactory<AppDbContext> dbFactory) : IUnmanagedSwitchProvider
{
    public async Task<IReadOnlyList<UnmanagedSwitch>> GetSwitchesAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entities = await db.UnmanagedSwitches.OrderBy(s => s.Name).ToListAsync(ct);
        return entities.Select(e => new UnmanagedSwitch(e.Id, e.Name, e.PortCount, e.Location, e.TagsCsv)).ToList();
    }

    public async Task SaveSwitchAsync(UnmanagedSwitch sw, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.UnmanagedSwitches.FirstOrDefaultAsync(s => s.Id == sw.Id, ct);

        if (entity is null)
        {
            db.UnmanagedSwitches.Add(new UnmanagedSwitchEntity
            {
                Id = sw.Id,
                Name = sw.Name,
                PortCount = sw.PortCount,
                Location = sw.Location,
                TagsCsv = sw.TagsCsv
            });
        }
        else
        {
            entity.Name = sw.Name;
            entity.PortCount = sw.PortCount;
            entity.Location = sw.Location;
            entity.TagsCsv = sw.TagsCsv;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteSwitchAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.UnmanagedSwitches.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is not null)
        {
            db.UnmanagedSwitches.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}