using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace NetAdmin.Infrastructure.Data;

public static class LocalDatabaseSeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static async Task SeedFromFileIfEmptyAsync(
        AppDbContext db,
        string? customFilePath = null,
        CancellationToken ct = default)
    {
        var seedFilePath = ResolveSeedFilePath(customFilePath);
        if (seedFilePath is null)
        {
            return;
        }

        await using var stream = File.OpenRead(seedFilePath);
        var payload = await JsonSerializer.DeserializeAsync<LocalSeedPayload>(stream, JsonOptions, ct);
        if (payload is null)
        {
            return;
        }

        var hasChanges = false;

        if (payload.HardwareDevices is { Count: > 0 } && !await db.HardwareDevices.AnyAsync(ct))
        {
            db.HardwareDevices.AddRange(payload.HardwareDevices);
            hasChanges = true;
        }

        if (payload.SwitchPorts is { Count: > 0 } && !await db.SwitchPorts.AnyAsync(ct))
        {
            db.SwitchPorts.AddRange(payload.SwitchPorts);
            hasChanges = true;
        }

        if (payload.PatchPanelPortItems is { Count: > 0 } && !await db.PatchPanelPortItems.AnyAsync(ct))
        {
            db.PatchPanelPortItems.AddRange(payload.PatchPanelPortItems);
            hasChanges = true;
        }

        if (payload.PhysicalLinks is { Count: > 0 } && !await db.PhysicalLinks.AnyAsync(ct))
        {
            db.PhysicalLinks.AddRange(payload.PhysicalLinks);
            hasChanges = true;
        }

        if (payload.Rooms is { Count: > 0 } && !await db.Rooms.AnyAsync(ct))
        {
            db.Rooms.AddRange(payload.Rooms);
            hasChanges = true;
        }

        if (hasChanges)
        {
            await db.SaveChangesAsync(ct);
        }
    }

    private static string? ResolveSeedFilePath(string? customFilePath)
    {
        string[] candidatePaths =
        [
            customFilePath ?? string.Empty,
            "seed.local.json",
            Path.Combine("data", "seed.local.json"),
            Path.Combine(AppContext.BaseDirectory, "seed.local.json")
        ];

        return candidatePaths.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p) && File.Exists(p));
    }

    private sealed class LocalSeedPayload
    {
        public List<HardwareDeviceEntity>? HardwareDevices { get; set; }
        public List<SwitchPortEntity>? SwitchPorts { get; set; }
        public List<PatchPanelPortItemEntity>? PatchPanelPortItems { get; set; }
        public List<PhysicalLinkEntity>? PhysicalLinks { get; set; }
        public List<RoomEntity>? Rooms { get; set; }
    }
}
