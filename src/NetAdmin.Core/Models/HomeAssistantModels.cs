using System.Text.Json;

namespace NetAdmin.Core.Models;

/// <summary>
/// Lightweight representation of a Home Assistant entity returned by the provider.
/// Includes a set of candidate identifiers (IPs, MACs, device ids) to help matching
/// against local NetAdmin hardware records.
/// </summary>
public record HomeAssistantEntity(
    string EntityId,
    string State,
    string? FriendlyName,
    string? DeviceClass,
    string? UnitOfMeasure,
    string AttributesJson,
    IReadOnlyList<string> CandidateDeviceIdentifiers);
