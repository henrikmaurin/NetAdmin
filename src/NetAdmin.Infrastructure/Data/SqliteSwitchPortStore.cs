using Microsoft.EntityFrameworkCore;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Infrastructure.Data;

public class SqliteSwitchPortStore(IDbContextFactory<AppDbContext> dbFactory) : ISwitchPortStore
{
    public Task<IReadOnlyList<SwitchPortDefinition>> GetPortsAsync(CancellationToken ct = default) =>
        GetPortsAsync("tplink-sg108pe-1", 8, ct);

    public async Task<IReadOnlyList<SwitchPortDefinition>> GetPortsAsync(string deviceId, int expectedPortCount, CancellationToken ct = default)
    {
        var normalizedDeviceId = string.IsNullOrWhiteSpace(deviceId) ? "tplink-sg108pe-1" : deviceId;
        var portCount = Math.Max(1, expectedPortCount);

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entities = await db.SwitchPorts
            .Where(p => p.DeviceId == normalizedDeviceId)
            .OrderBy(p => p.PortNumber)
            .ToListAsync(ct);

        var entityByPort = entities.ToDictionary(e => e.PortNumber);
        var result = new List<SwitchPortDefinition>(portCount);

        for (var portNum = 1; portNum <= portCount; portNum++)
        {
            if (entityByPort.TryGetValue(portNum, out var entity))
            {
                var tagged = string.IsNullOrWhiteSpace(entity.TaggedVlanIdsCsv)
                    ? []
                    : entity.TaggedVlanIdsCsv
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(s => int.TryParse(s, out var v) ? v : 0)
                        .Where(v => v > 0)
                        .ToList();

                result.Add(new SwitchPortDefinition(
                    PortNumber: entity.PortNumber,
                    Label: entity.Label,
                    Role: entity.Role,
                    NativeVlanId: entity.NativeVlanId,
                    TaggedVlanIds: tagged,
                    PoeEnabled: entity.PoeEnabled,
                    DeviceId: normalizedDeviceId));
            }
            else
            {
                result.Add(new SwitchPortDefinition(
                    PortNumber: portNum,
                    Label: $"Port {portNum}",
                    Role: SwitchPortRole.Unused,
                    NativeVlanId: 1,
                    TaggedVlanIds: [],
                    PoeEnabled: false,
                    DeviceId: normalizedDeviceId));
            }
        }

        return result;
    }

    public async Task SavePortAsync(SwitchPortDefinition port, CancellationToken ct = default)
    {
        var normalizedDeviceId = string.IsNullOrWhiteSpace(port.DeviceId) ? "tplink-sg108pe-1" : port.DeviceId;

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var entity = await db.SwitchPorts
            .FirstOrDefaultAsync(p => p.DeviceId == normalizedDeviceId && p.PortNumber == port.PortNumber, ct);

        var taggedCsv = string.Join(",", port.TaggedVlanIds.Distinct().OrderBy(x => x));
        var nativeVlan = port.NativeVlanId <= 0 ? 1 : port.NativeVlanId;

        if (entity is null)
        {
            db.SwitchPorts.Add(new SwitchPortEntity
            {
                DeviceId = normalizedDeviceId,
                PortNumber = port.PortNumber,
                Label = port.Label,
                Role = port.Role,
                NativeVlanId = nativeVlan,
                TaggedVlanIdsCsv = taggedCsv,
                PoeEnabled = port.PoeEnabled
            });
        }
        else
        {
            entity.Label = port.Label;
            entity.Role = port.Role;
            entity.NativeVlanId = nativeVlan;
            entity.TaggedVlanIdsCsv = taggedCsv;
            entity.PoeEnabled = port.PoeEnabled;
        }

        await db.SaveChangesAsync(ct);
    }
}