using System.Text.Json;

namespace NetAdmin.Core.Models;

/// <summary>
/// Lightweight representation of a Home Assistant entity returned by the provider.
/// Candidate device identifiers contain network addresses used to match local hardware.
/// </summary>
public record HomeAssistantEntity(
    string EntityId,
    string State,
    string? FriendlyName,
    string? DeviceClass,
    string? UnitOfMeasure,
    string AttributesJson,
    IReadOnlyList<string> CandidateDeviceIdentifiers,
    bool IsShellyDevice = false);

public record HomeAssistantDevice(
    string Id,
    string Name,
    string? Manufacturer,
    string? Model,
    IReadOnlyList<string> CandidateDeviceIdentifiers,
    IReadOnlyList<string> EntityIds,
    bool IsShellyDevice = false);
