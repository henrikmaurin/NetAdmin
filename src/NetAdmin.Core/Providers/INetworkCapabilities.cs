using NetAdmin.Core.Models;

namespace NetAdmin.Core.Providers;

public interface INetworkIntegration
{
    string ProviderName { get; }
    Task<bool> TestConnectionAsync(CancellationToken ct = default);
}

public interface IRoutingProvider : INetworkIntegration
{
    Task<IReadOnlyList<NetworkInterface>> GetInterfacesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<VlanDefinition>> GetVlansAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ArpEntry>> GetArpTableAsync(CancellationToken ct = default);
}

public interface IFirewallProvider : INetworkIntegration
{
    Task<IReadOnlyList<FirewallRule>> GetRulesAsync(CancellationToken ct = default);
    Task<bool> BlockHostAsync(string ipOrMac, CancellationToken ct = default);
}

public interface IDhcpProvider : INetworkIntegration
{
    Task<IReadOnlyList<DhcpLease>> GetLeasesAsync(CancellationToken ct = default);
    Task<bool> AddStaticLeaseAsync(string ipAddress, string macAddress, string hostname, string? description = null, CancellationToken ct = default);
}
public interface IWirelessProvider : INetworkIntegration
{
    Task<IReadOnlyList<AccessPoint>> GetAccessPointsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<WirelessNetwork>> GetWirelessNetworksAsync(CancellationToken ct = default);
    Task<IReadOnlyList<WirelessClient>> GetWirelessClientsAsync(CancellationToken ct = default);
    Task<bool> RestartAccessPointAsync(string apMacAddress, CancellationToken ct = default);
}

public interface ISwitchProvider : INetworkIntegration
{
    Task<IReadOnlyList<HardwareDevice>> GetConfigurableDevicesAsync(CancellationToken ct = default);
    Task<SwitchDevice> GetSwitchAsync(CancellationToken ct = default);
    Task<SwitchDevice> GetSwitchAsync(string deviceId, CancellationToken ct = default);
    Task UpdatePortAsync(SwitchPortDefinition port, CancellationToken ct = default);
    Task<SwitchConfigurationPlan> GeneratePlaybookAsync(
        IReadOnlyList<VlanDefinition> routerVlans,
        IReadOnlyList<WirelessNetwork> wifiNetworks,
        CancellationToken ct = default);
    Task<SwitchConfigurationPlan> GeneratePlaybookAsync(
        string deviceId,
        IReadOnlyList<VlanDefinition> routerVlans,
        IReadOnlyList<WirelessNetwork> wifiNetworks,
        CancellationToken ct = default);
}

public interface ISwitchPortStore
{
    Task<IReadOnlyList<SwitchPortDefinition>> GetPortsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SwitchPortDefinition>> GetPortsAsync(string deviceId, int expectedPortCount, CancellationToken ct = default);
    Task SavePortAsync(SwitchPortDefinition port, CancellationToken ct = default);
}

public record ShellyDevice(
    string Id,
    string Name,
    string IpAddress,
    string Model,
    string Room,
    string TagsCsv = "") : ITaggableEntity;

public interface IShellyStore
{
    Task<IReadOnlyList<ShellyDevice>> GetDevicesAsync(CancellationToken ct = default);
    Task SaveDeviceAsync(ShellyDevice device, CancellationToken ct = default);
    Task DeleteDeviceAsync(string id, CancellationToken ct = default);
}

public interface IShellyProvider : INetworkIntegration
{
    Task<IReadOnlyList<ShellyDevice>> GetDevicesAsync(CancellationToken ct = default);
    Task SaveDeviceAsync(ShellyDevice device, CancellationToken ct = default);
    Task DeleteDeviceAsync(string id, CancellationToken ct = default);
    Task<ShellyProbedInfo?> ProbeDeviceAsync(string ipAddress, CancellationToken ct = default);
}

public record HomeWizardProbedInfo(
    string IpAddress,
    string ProductName,
    string ProductTypeCode,
    string? SerialHex,
    string? FirmwareVersion,
    string? ApiVersion,
    HardwareProductDefinition? MatchedProduct);

public interface IHomeWizardProvider : INetworkIntegration
{
    Task<HomeWizardProbedInfo?> ProbeDeviceAsync(string ipAddress, CancellationToken ct = default);
}

public interface IIkeaProvider : INetworkIntegration
{
}

public record FrigateCameraSummary(
    string Name,
    double CameraFps,
    double DetectionFps,
    bool DetectionEnabled);

public record FrigateStatusSummary(
    string Version,
    int CameraCount,
    IReadOnlyList<string> Detectors,
    IReadOnlyList<FrigateCameraSummary> Cameras);

public interface IFrigateProvider : INetworkIntegration
{
    Task<FrigateStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default);

    // Add or remove cameras via a go2rtc instance. Implementations should
    // resolve an optional go2rtc base URL from device settings (e.g. "go2rtc:BaseUrl")
    // or global settings (e.g. "Go2Rtc:BaseUrl") when go2rtcBaseUrl is null.
    Task<bool> AddCameraViaGo2RtcAsync(string? deviceId, string cameraName, string rtspUrl, string? go2rtcBaseUrl = null, CancellationToken ct = default);
    Task<bool> RemoveCameraViaGo2RtcAsync(string? deviceId, string cameraName, string? go2rtcBaseUrl = null, CancellationToken ct = default);

    // Add or remove a camera entry in Frigate's configuration via the Frigate API.
    // Implementations should use the same Frigate base URL and api token used for GetStatusAsync
    // when frigateBaseUrl/apiToken are null.
    Task<bool> AddCameraToFrigateAsync(string? deviceId, string cameraName, string rtspUrl, string? frigateBaseUrl = null, CancellationToken ct = default);
    Task<bool> RemoveCameraFromFrigateAsync(string? deviceId, string cameraName, string? frigateBaseUrl = null, CancellationToken ct = default);
    Task<IReadOnlyDictionary<string, string?>> GetCameraSourcesAsync(string? deviceId = null, string? frigateBaseUrl = null, CancellationToken ct = default);
}

public record HomeAssistantStatusSummary(
    string Version,
    string LocationName,
    string State,
    int ComponentCount,
    int EntityCount);

public interface IHomeAssistantProvider : INetworkIntegration
{
    Task<HomeAssistantStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default);

    /// <summary>
    /// Retrieve the list of entities from a Home Assistant instance. The returned
    /// HomeAssistantEntity objects include a small set of candidate identifiers
    /// (ip, ip_address, mac, mac_address, device_id) extracted from attributes to
    /// assist downstream matching to NetAdmin hardware entries.
    /// </summary>
    Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantEntity>> GetEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default);
}

public record PatchPanelPort(
    int PortNumber,
    string Label,
    string Room,
    string TagsCsv = "") : ITaggableEntity
{
    public string Id => $"patch-port-{PortNumber}";
    public string Name => $"Patchport {PortNumber} ({Label})";
}

public interface IPatchPanelProvider
{
    Task<IReadOnlyList<PatchPanelPort>> GetPortsAsync(CancellationToken ct = default);
    Task SavePortAsync(PatchPanelPort port, CancellationToken ct = default);
    Task SetPortCountAsync(int portCount, CancellationToken ct = default);
}

public record UnmanagedSwitch(
    string Id,
    string Name,
    int PortCount,
    string Location,
    string TagsCsv = "") : ITaggableEntity;

public interface IUnmanagedSwitchProvider
{
    Task<IReadOnlyList<UnmanagedSwitch>> GetSwitchesAsync(CancellationToken ct = default);
    Task SaveSwitchAsync(UnmanagedSwitch sw, CancellationToken ct = default);
    Task DeleteSwitchAsync(string id, CancellationToken ct = default);
}

public record PhysicalLink(
    string Id,
    string SourceDeviceId,
    string SourceDeviceName,
    int SourcePort,
    string TargetDeviceId,
    string TargetDeviceName,
    int TargetPort,
    string CableLabel = "", // T.ex. "Cat6 Blå", "Vägguttag Kontor"
    string TagsCsv = "") : ITaggableEntity
{
    public string Name => $"{SourceDeviceName} (Port {SourcePort}) ⟷ {TargetDeviceName} (Port {TargetPort})";
}

public interface IPhysicalLinkProvider
{
    Task<IReadOnlyList<PhysicalLink>> GetLinksAsync(CancellationToken ct = default);
    Task SaveLinkAsync(PhysicalLink link, CancellationToken ct = default);
    Task DeleteLinkAsync(string id, CancellationToken ct = default);
}

public record KnownDeviceMetadata(
    string MacAddress,
    string CustomName,
    string Room,
    string Notes,
    string TagsCsv = "") : ITaggableEntity
{
    public string Id => MacAddress;
    public string Name => CustomName;
}

public record RoomItem(
    string Id,
    string Name,
    string BuildingOrZone,
    string Floor,
    string TagsCsv = "") : ITaggableEntity;

public interface IRoomProvider
{
    Task<IReadOnlyList<RoomItem>> GetRoomsAsync(CancellationToken ct = default);
    Task SaveRoomAsync(RoomItem room, CancellationToken ct = default);
    Task DeleteRoomAsync(string id, CancellationToken ct = default);
}

/// <summary>
/// Optional provider interface for logical services (Home Assistant, Frigate, NVR, etc.).
/// Providers that can operate on services may implement this to provide status/test endpoints.
/// </summary>
public interface IServiceIntegration : INetworkIntegration
{
    /// <summary>
    /// Get a normalized service status for the given service identifier or base URL.
    /// Returns null when the provider cannot reach or identify the service.
    /// </summary>
    Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, CancellationToken ct = default);
}
