using NetAdmin.Core.Providers;

namespace NetAdmin.Core.Models;

public enum HardwareCategory
{
    Generic = 0,
    IoT = 1,
    Switch = 2,
    PatchPanel = 3,
    Router = 4,
    AccessPoint = 5,
    FiberConverter = 6,
    WanEndpoint = 7,
    Camera = 8,
    Server = 9,
    IoTHub = 10,
    EnergyMeter = 11,
    Phone = 12,
    Tablet = 13,
    Tv = 14,
    Computer = 15,
    MediaPlayer = 16,
    SmartSpeaker = 17,
    Soundbar = 18,
    Printer = 19,
    Nvr = 20,
    HomeAutomation = 21,
    SmartWatch = 22
}

public static class HardwareCategoryExtensions
{
    public static string ToDisplayName(this HardwareCategory category) => category switch
    {
        HardwareCategory.Generic => "Generic",
        HardwareCategory.IoT => "IoT",
        HardwareCategory.Switch => "Switch",
        HardwareCategory.PatchPanel => "Patch Panel",
        HardwareCategory.Router => "Router",
        HardwareCategory.AccessPoint => "Access Point",
        HardwareCategory.FiberConverter => "Fiber Converter",
        HardwareCategory.WanEndpoint => "WAN Endpoint",
        HardwareCategory.Camera => "Camera",
        HardwareCategory.Server => "Server",
        HardwareCategory.IoTHub => "IoT Hub",
        HardwareCategory.EnergyMeter => "Energy Meter",
        HardwareCategory.Phone => "Phone",
        HardwareCategory.Tablet => "Tablet",
        HardwareCategory.Tv => "TV",
        HardwareCategory.Computer => "Computer",
        HardwareCategory.MediaPlayer => "Media Player",
        HardwareCategory.SmartSpeaker => "Smart Speaker",
        HardwareCategory.Soundbar => "Soundbar",
        HardwareCategory.Printer => "Printer",
        HardwareCategory.Nvr => "NVR",
        HardwareCategory.HomeAutomation => "Home Automation",
        HardwareCategory.SmartWatch => "Smart Watch",
        _ => category.ToString()
    };
}

public record HardwareDevice(
    string Id,
    string Name,
    HardwareCategory Category,
    string? IpAddress,
    string? MacAddress,
    string Room,
    int PortCount,
    string? ManagementUrl,
    string Notes,
    string TagsCsv = "",
    string Vendor = "",
    string Model = "") : ITaggableEntity
{
    public virtual string EffectiveManagementUrl =>
        !string.IsNullOrWhiteSpace(ManagementUrl)
            ? ManagementUrl
            : (!string.IsNullOrWhiteSpace(IpAddress) && Category is not (HardwareCategory.WanEndpoint or HardwareCategory.FiberConverter)
                ? $"http://{IpAddress}"
                : string.Empty);
}

public record GenericNetworkDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    int PortCount = 1,
    string? ManagementUrl = null,
    string Notes = "",
    string TagsCsv = "",
    string Vendor = "",
    string Model = "")
    : HardwareDevice(Id, Name, HardwareCategory.Generic, IpAddress, MacAddress, Room, PortCount, ManagementUrl, Notes, TagsCsv, Vendor, Model);

public record ShellyHardwareDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    string ShellyModel = "Plus 1PM",
    int Generation = 2,
    string Notes = "",
    string TagsCsv = "shelly, iot")
    : HardwareDevice(Id, Name, HardwareCategory.IoT, IpAddress, MacAddress, Room, 0, !string.IsNullOrWhiteSpace(IpAddress) ? $"http://{IpAddress}" : null, Notes, TagsCsv, "Shelly", ShellyModel);

public record SwitchHardwareDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    int PortCount = 8,
    bool IsManaged = false,
    string Vendor = "Generic",
    string SwitchModel = "Unmanaged",
    string? ManagementUrl = null,
    string Notes = "",
    string TagsCsv = "switch")
    : HardwareDevice(Id, Name, HardwareCategory.Switch, IpAddress, MacAddress, Room, PortCount, ManagementUrl, Notes, TagsCsv, Vendor, SwitchModel);

public record PatchPanelHardwareDevice(
    string Id,
    string Name,
    string Room,
    int PortCount = 8,
    string Notes = "",
    string TagsCsv = "patchpanel",
    IReadOnlyList<PatchPanelPortItem>? Ports = null)
    : HardwareDevice(Id, Name, HardwareCategory.PatchPanel, null, null, Room, PortCount, null, Notes, TagsCsv, "Generic", $"{PortCount}-Port Patch Panel");
// NY: Fiberswitch / Tjänstefördelare (CPE / ONT)
public record FiberConverterHardwareDevice(
    string Id,
    string Name,
    string Room,
    int PortCount = 5,
    string Vendor = "Stadsnät",
    string ConverterModel = "CPE / ONT",
    string Notes = "",
    string TagsCsv = "wan, fiber, cpe")
    : HardwareDevice(Id, Name, HardwareCategory.FiberConverter, null, null, Room, PortCount, null, Notes, TagsCsv, Vendor, ConverterModel);
// NY: Internet / Operatör (WAN Endpoint)
public record WanHardwareDevice(
    string Id,
    string Name,
    string? PublicIpAddress = null,
    string IspName = "ISP",
    string Notes = "Fiber WAN",
    string TagsCsv = "internet, wan, isp")
    : HardwareDevice(Id, Name, HardwareCategory.WanEndpoint, PublicIpAddress, null, "Externt (WAN)", 1, null, Notes, TagsCsv, IspName, "WAN");
public record PatchPanelPortItem(
    string PatchPanelId,
    int PortNumber,
    string Label,
    string Room,
    string TagsCsv = "");

public record CameraStreamDefinition(
    string Name,
    string Url,
    string Protocol = "RTSP",
    string Resolution = "");

public record CameraHardwareDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    string Vendor = "Reolink",
    string Model = "",
    int PortCount = 1,
    string? RtspPath = null,
    string Notes = "",
    string TagsCsv = "camera, frigate",
    IReadOnlyList<CameraStreamDefinition>? Streams = null)
    : HardwareDevice(Id, Name, HardwareCategory.Camera, IpAddress, MacAddress, Room, PortCount, !string.IsNullOrWhiteSpace(IpAddress) ? $"http://{IpAddress}" : null, Notes, TagsCsv, Vendor, Model)
{
    public IReadOnlyList<CameraStreamDefinition> EffectiveStreams =>
        Streams is { Count: > 0 }
            ? Streams
            : (!string.IsNullOrWhiteSpace(RtspPath)
                ? [new CameraStreamDefinition("Main Stream", RtspPath, "RTSP")]
                : []);
}

/// <summary>
/// IoT-hubb / Smart Hem-gateway (t.ex. IKEA TRÅDFRI, DIRIGERA, Hue Bridge, Homey)
/// där man kan specificera vilka protokoll/tekniker som stöds (Zigbee, Matter, Thread, BLE, Z-Wave, 433MHz, LAN, Wi-Fi).
/// </summary>
public record IoTHubHardwareDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    string Vendor = "IKEA",
    string HubModel = "TRÅDFRI Gateway",
    int PortCount = 1,
    string SupportedProtocolsCsv = "Zigbee, Ethernet",
    string? ManagementUrl = null,
    string Notes = "",
    string TagsCsv = "iothub, zigbee, lan")
    : HardwareDevice(Id, Name, HardwareCategory.IoTHub, IpAddress, MacAddress, Room, PortCount, ManagementUrl, Notes, TagsCsv, Vendor, HubModel);

/// <summary>
/// Dedikerad energimätare (t.ex. HomeWizard Wi-Fi P1 Meter, HomeWizard kWh Meter, Shelly Pro 3EM)
/// med information om mätgränssnitt/faser (RJ12 P1/HAN, 3-fas CT, 1-fas DIN, etc.).
/// </summary>
public record EnergyMeterHardwareDevice(
    string Id,
    string Name,
    string? IpAddress,
    string? MacAddress,
    string Room,
    string Vendor = "HomeWizard",
    string MeterModel = "Wi-Fi P1 Meter",
    int PortCount = 0,
    string MeterInterface = "RJ12 P1 / HAN",
    string? ManagementUrl = null,
    string Notes = "",
    string TagsCsv = "energymeter, p1-meter, han")
    : HardwareDevice(Id, Name, HardwareCategory.EnergyMeter, IpAddress, MacAddress, Room, PortCount, ManagementUrl, Notes, TagsCsv, Vendor, MeterModel);

public interface IHardwareInventoryProvider
{
    Task<IReadOnlyList<HardwareDevice>> GetAllHardwareAsync(CancellationToken ct = default);
    Task<HardwareDevice?> GetHardwareByIdAsync(string id, CancellationToken ct = default);
    Task<HardwareDevice?> GetHardwareByMacAsync(string macAddress, CancellationToken ct = default);
    Task SaveHardwareAsync(HardwareDevice device, CancellationToken ct = default);
    Task DeleteHardwareAsync(string id, CancellationToken ct = default);

    Task<IReadOnlyList<PatchPanelPortItem>> GetPatchPanelPortsAsync(string patchPanelId, CancellationToken ct = default);
    Task SavePatchPanelPortAsync(PatchPanelPortItem port, CancellationToken ct = default);
}
