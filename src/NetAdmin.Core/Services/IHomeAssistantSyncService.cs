using NetAdmin.Core.Models;

namespace NetAdmin.Core.Services;

public record HomeAssistantSyncResult(
    string ServiceId,
    string EntityId,
    string Name,
    string? LinkedDeviceId,
    string? LinkedDeviceName,
    IReadOnlyList<string> CandidateIdentifiers,
    string AttributesJson);

public interface IHomeAssistantSyncService
{
    /// <summary>
    /// Sync entities from the specified Home Assistant instance and upsert corresponding
    /// Service records. Returns a list of results describing matched devices.
    /// </summary>
    Task<IReadOnlyList<HomeAssistantSyncResult>> SyncEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default);

    /// <summary>
    /// Set or clear the linked device id for a previously synced service.
    /// </summary>
    Task SetServiceLinkAsync(string serviceId, string? linkedDeviceId, CancellationToken ct = default);
}
