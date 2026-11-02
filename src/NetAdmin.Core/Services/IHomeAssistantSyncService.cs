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
    /// Sync all entities from the specified Home Assistant instance and upsert corresponding
    /// service records. Each result includes any automatically matched hardware device.
    /// </summary>
    Task<IReadOnlyList<HomeAssistantSyncResult>> SyncEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default);

    Task<IReadOnlyList<HomeAssistantSyncResult>> SyncServiceEntitiesAsync(string serviceId, CancellationToken ct = default);

    /// <summary>
    /// Set or clear the linked device id for a previously synced service.
    /// </summary>
    Task SetServiceLinkAsync(string serviceId, string? linkedDeviceId, CancellationToken ct = default);
}
