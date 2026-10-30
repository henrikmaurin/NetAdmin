using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.Ikea.Components;

namespace NetAdmin.Providers.Ikea;

public class IoTHubHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "iothub";
    public string DisplayName => "📡 IoT Hub";
    public string Icon => "📡";
    public HardwareCategory Category => HardwareCategory.IoTHub;
    public int Priority => 80;

    public Type? EditorComponentType => typeof(IoTHubEditor);
    public Type? CardSummaryComponentType => typeof(IoTHubCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is IoTHubHardwareDevice || device.Category == HardwareCategory.IoTHub;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.IoTHub ||
        (vendor?.Equals("IKEA", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("tradfri", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("dirigera", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("lumi-gateway", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("zndmwg", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.StartsWith("gw-", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new IoTHubHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New IoT Hub" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            Vendor: "IKEA",
            HubModel: "TRÅDFRI Gateway (E1526)",
            PortCount: 1,
            SupportedProtocolsCsv: "Zigbee, Ethernet (LAN), CoAP",
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "iothub, ikea, tradfri, zigbee, lan");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is IoTHubHardwareDevice hub)
        {
            return hub with { Category = HardwareCategory.IoTHub, Model = hub.HubModel };
        }

        return new IoTHubHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            Vendor: string.IsNullOrWhiteSpace(current.Vendor) ? "IKEA" : current.Vendor,
            HubModel: string.IsNullOrWhiteSpace(current.Model) ? "TRÅDFRI Gateway (E1526)" : current.Model,
            PortCount: current.PortCount <= 0 ? 1 : current.PortCount,
            SupportedProtocolsCsv: "Zigbee, Ethernet (LAN)",
            ManagementUrl: current.ManagementUrl,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "iothub, ikea, zigbee, lan" : current.TagsCsv);
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("api:token", "Hub API Token / Security Code", "DIRIGERA Bearer Token / TRÅDFRI Security Code / Bridge Key", IsSecret: true),
        new("api:identity", "Client Identity / App Username (Optional)", "CoAP identity or local user")
    ];
}
