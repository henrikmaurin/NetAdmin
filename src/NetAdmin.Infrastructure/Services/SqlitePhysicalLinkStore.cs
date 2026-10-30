using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class SqlitePhysicalLinkStore(IDbContextFactory<AppDbContext> dbFactory) : IPhysicalLinkProvider
{
    public async Task<IReadOnlyList<PhysicalLink>> GetLinksAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var list = await db.PhysicalLinks
            .OrderBy(l => l.SourceDeviceName)
            .ThenBy(l => l.SourcePort)
            .ToListAsync(ct);

        return list.Select(e => new PhysicalLink(
            e.Id,
            e.SourceDeviceId,
            e.SourceDeviceName,
            e.SourcePort,
            e.TargetDeviceId,
            e.TargetDeviceName,
            e.TargetPort,
            e.CableLabel,
            e.TagsCsv)).ToList();
    }

    public async Task SaveLinkAsync(PhysicalLink link, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.PhysicalLinks.FirstOrDefaultAsync(l => l.Id == link.Id, ct);

        if (entity is null)
        {
            db.PhysicalLinks.Add(new PhysicalLinkEntity
            {
                Id = link.Id,
                SourceDeviceId = link.SourceDeviceId,
                SourceDeviceName = link.SourceDeviceName,
                SourcePort = link.SourcePort,
                TargetDeviceId = link.TargetDeviceId,
                TargetDeviceName = link.TargetDeviceName,
                TargetPort = link.TargetPort,
                CableLabel = link.CableLabel,
                TagsCsv = link.TagsCsv
            });
        }
        else
        {
            entity.SourceDeviceId = link.SourceDeviceId;
            entity.SourceDeviceName = link.SourceDeviceName;
            entity.SourcePort = link.SourcePort;
            entity.TargetDeviceId = link.TargetDeviceId;
            entity.TargetDeviceName = link.TargetDeviceName;
            entity.TargetPort = link.TargetPort;
            entity.CableLabel = link.CableLabel;
            entity.TagsCsv = link.TagsCsv;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteLinkAsync(string id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.PhysicalLinks.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (entity is not null)
        {
            db.PhysicalLinks.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}