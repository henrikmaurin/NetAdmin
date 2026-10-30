using NetAdmin.Core.Components;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.TpLink.Components;

namespace NetAdmin.Providers.TpLink;

public class OmadaControllerHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "omada-controller";
    public string DisplayName => "📡 Omada Controller";
    public string Icon => "📡";
    public HardwareCategory Category => HardwareCategory.Router;
    public int Priority => 60;

    public Type? EditorComponentType => typeof(CommonRouterEditor);
    public Type? HeaderActionsComponentType => typeof(OmadaHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Router &&
        ((device.Vendor?.Contains("omada", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (device.TagsCsv?.Contains("omada", StringComparison.OrdinalIgnoreCase) ?? false));

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Router &&
        ((vendor?.Contains("omada", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (tagsCsv?.Contains("omada", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (hostname?.Contains("omada", StringComparison.OrdinalIgnoreCase) ?? false));

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "Omada Controller" : name,
            Category: HardwareCategory.Router,
            IpAddress: ipAddress ?? "192.168.1.2",
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 2,
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"https://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "controller, omada",
            Vendor: "Omada",
            Model: "Omada");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Router,
            PortCount = current.PortCount <= 0 ? 2 : current.PortCount
        };

    public IEnumerable<NetAdmin.Core.Models.DeviceCapability> GetCapabilities(HardwareDevice device)
    {
        var caps = new List<NetAdmin.Core.Models.DeviceCapability>();
        if (!string.IsNullOrWhiteSpace(device.IpAddress)) caps.Add(NetAdmin.Core.Models.DeviceCapability.WebGui);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.ControllerConfig);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Probe);
        return caps;
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("api:username", "Omada Controller Username", "admin"),
        new("api:password", "Omada Controller Password", "Controller password", IsSecret: true),
        new("api:clientId", "OpenAPI Client ID (Optional)", "Omada OpenAPI Client ID"),
        new("api:clientSecret", "OpenAPI Client Secret (Optional)", "Omada OpenAPI Secret", IsSecret: true)
    ];
}
