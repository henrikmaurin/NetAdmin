using Microsoft.EntityFrameworkCore;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public sealed class SqliteLinkedServiceProvider(IDbContextFactory<AppDbContext> dbFactory) : ILinkedServiceProvider
{
    public async Task<IReadOnlyList<LinkedServiceSummary>> GetLinkedServicesAsync(string deviceId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("A device ID is required.", nameof(deviceId));

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Services
            .AsNoTracking()
            .Where(service => service.LinkedDeviceId == deviceId)
            .OrderBy(service => service.Name)
            .Select(service => new LinkedServiceSummary(
                service.Id,
                service.Name,
                service.Type,
                service.LinkedDeviceId,
                service.ParentServiceId,
                service.ParentServiceId == null
                    ? null
                    : db.Services.Where(parent => parent.Id == service.ParentServiceId).Select(parent => parent.LinkedDeviceId).FirstOrDefault(),
                service.IsServiceInstance))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<LinkedServiceSummary>> GetAvailableHomeAssistantServicesAsync(string homeAssistantDeviceId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(homeAssistantDeviceId))
            throw new ArgumentException("A Home Assistant device ID is required.", nameof(homeAssistantDeviceId));

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var instanceServiceId = await GetInstanceServiceIdAsync(db, homeAssistantDeviceId, ct);
        if (instanceServiceId is null)
            return [];

        return await db.Services
            .AsNoTracking()
            .Where(service => service.Id.StartsWith("homeassistant:") &&
                              service.ParentServiceId == instanceServiceId &&
                              service.LinkedDeviceId == null)
            .OrderBy(service => service.Name)
            .Select(service => new LinkedServiceSummary(
                service.Id,
                service.Name,
                service.Type,
                service.LinkedDeviceId,
                service.ParentServiceId,
                service.ParentServiceId == null
                    ? null
                    : db.Services.Where(parent => parent.Id == service.ParentServiceId).Select(parent => parent.LinkedDeviceId).FirstOrDefault(),
                service.IsServiceInstance))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<LinkedServiceSummary>> GetHomeAssistantEntitiesAsync(string homeAssistantDeviceId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(homeAssistantDeviceId))
            throw new ArgumentException("A Home Assistant device ID is required.", nameof(homeAssistantDeviceId));

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var instanceServiceId = await GetInstanceServiceIdAsync(db, homeAssistantDeviceId, ct);
        if (instanceServiceId is null)
            return [];

        return await db.Services
            .AsNoTracking()
            .Where(service => service.ParentServiceId == instanceServiceId)
            .OrderBy(service => service.Name)
            .Select(service => new LinkedServiceSummary(
                service.Id,
                service.Name,
                service.Type,
                service.LinkedDeviceId,
                service.ParentServiceId,
                service.ParentServiceId == null
                    ? null
                    : db.Services.Where(parent => parent.Id == service.ParentServiceId).Select(parent => parent.LinkedDeviceId).FirstOrDefault(),
                service.IsServiceInstance))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<LinkedServiceSummary>>> GetLinkedServicesForDevicesAsync(IReadOnlyCollection<string> deviceIds, CancellationToken ct = default)
    {
        if (deviceIds.Count == 0)
            return new Dictionary<string, IReadOnlyList<LinkedServiceSummary>>(StringComparer.OrdinalIgnoreCase);

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var links = await db.Services
            .AsNoTracking()
            .Where(service => service.LinkedDeviceId != null && deviceIds.Contains(service.LinkedDeviceId))
            .OrderBy(service => service.Name)
            .Select(service => new LinkedServiceSummary(
                service.Id,
                service.Name,
                service.Type,
                service.LinkedDeviceId,
                service.ParentServiceId,
                service.ParentServiceId == null
                    ? null
                    : db.Services.Where(parent => parent.Id == service.ParentServiceId).Select(parent => parent.LinkedDeviceId).FirstOrDefault(),
                service.IsServiceInstance))
            .ToListAsync(ct);

        return links
            .GroupBy(link => link.LinkedDeviceId!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<LinkedServiceSummary>)group.ToArray(), StringComparer.OrdinalIgnoreCase);
    }

    private static async Task<string?> GetInstanceServiceIdAsync(AppDbContext db, string deviceId, CancellationToken ct)
    {
        var directInstanceId = await db.Services
            .AsNoTracking()
            .Where(service => service.Id == deviceId && service.IsServiceInstance)
            .Select(service => service.Id)
            .FirstOrDefaultAsync(ct);
        if (directInstanceId is not null)
            return directInstanceId;

        var instanceIds = await db.Services
            .AsNoTracking()
            .Where(service => service.LinkedDeviceId == deviceId && service.IsServiceInstance)
            .Select(service => service.Id)
            .ToListAsync(ct);

        return instanceIds.FirstOrDefault(HomeAssistantServiceIds.IsInstanceServiceId) ?? instanceIds.FirstOrDefault();
    }
}
