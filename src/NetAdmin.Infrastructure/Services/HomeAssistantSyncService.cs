using Microsoft.EntityFrameworkCore;
using NetAdmin.Core.Models;
using NetAdmin.Core.Services;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class HomeAssistantSyncService : IHomeAssistantSyncService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly IHomeAssistantProvider _haProvider;

    public HomeAssistantSyncService(IDbContextFactory<AppDbContext> dbFactory, IHomeAssistantProvider haProvider)
    {
        _dbFactory = dbFactory;
        _haProvider = haProvider;
    }

    public async Task<IReadOnlyList<HomeAssistantSyncResult>> SyncEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default)
    {
        var entities = await _haProvider.GetEntitiesAsync(deviceId, ipOrUrl, ct);
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var results = new List<HomeAssistantSyncResult>();

        foreach (var e in entities)
        {
            ct.ThrowIfCancellationRequested();

            var svcId = $"homeassistant:{e.EntityId}";

            // Attempt to match candidate identifiers to known hardware devices
            string? matchedDeviceId = null;
            string? matchedDeviceName = null;

            foreach (var cand in e.CandidateDeviceIdentifiers)
            {
                if (string.IsNullOrWhiteSpace(cand)) continue;

                var cleaned = cand.Trim();

                // IP match
                var byIp = await db.HardwareDevices.FirstOrDefaultAsync(h => h.IpAddress == cleaned, ct);
                if (byIp is not null)
                {
                    matchedDeviceId = byIp.Id;
                    matchedDeviceName = byIp.Name;
                    break;
                }

                // MAC match - normalize
                var normalized = cleaned.Replace("-", "").Replace(":", "").Replace(".", "").ToLowerInvariant();
                var byMac = await db.HardwareDevices.FirstOrDefaultAsync(h => (h.MacAddress ?? "").Replace("-", "").Replace(":", "").Replace(".", "").ToLower() == normalized, ct);
                if (byMac is not null)
                {
                    matchedDeviceId = byMac.Id;
                    matchedDeviceName = byMac.Name;
                    break;
                }
            }

            // Upsert service record
            var svc = await db.Services.FirstOrDefaultAsync(s => s.Id == svcId, ct);
            var name = e.FriendlyName ?? e.EntityId;
            var description = e.AttributesJson;
            var type = ServiceType.HomeAutomation.ToString();

            if (svc is null)
            {
                svc = new NetAdmin.Infrastructure.Data.ServiceEntity
                {
                    Id = svcId,
                    Name = name,
                    Type = type,
                    Description = description,
                    LinkedDeviceId = matchedDeviceId
                };
                db.Services.Add(svc);
            }
            else
            {
                svc.Name = name;
                svc.Description = description;
                svc.Type = type;
                svc.LinkedDeviceId = matchedDeviceId;
                db.Services.Update(svc);
            }

            await db.SaveChangesAsync(ct);

            results.Add(new HomeAssistantSyncResult(svc.Id, e.EntityId, name, matchedDeviceId, matchedDeviceName, e.CandidateDeviceIdentifiers, e.AttributesJson));
        }

        return results;
    }

    public async Task SetServiceLinkAsync(string serviceId, string? linkedDeviceId, CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        var svc = await db.Services.FirstOrDefaultAsync(s => s.Id == serviceId, ct);
        if (svc is null) return;
        svc.LinkedDeviceId = linkedDeviceId;
        db.Services.Update(svc);
        await db.SaveChangesAsync(ct);
    }
}
