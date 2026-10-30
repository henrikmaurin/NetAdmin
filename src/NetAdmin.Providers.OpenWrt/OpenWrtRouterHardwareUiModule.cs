using NetAdmin.Core.Components;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.OpenWrt.Components;

namespace NetAdmin.Providers.OpenWrt;

public class OpenWrtRouterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "openwrt-router";
    public string DisplayName => "🐧 OpenWrt Router";
    public string Icon => "🐧";
    public HardwareCategory Category => HardwareCategory.Router;
    public int Priority => 60;

    public Type? EditorComponentType => typeof(CommonRouterEditor);
    public Type? HeaderActionsComponentType => typeof(OpenWrtHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Router &&
        ((device.Vendor?.Contains("openwrt", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (device.TagsCsv?.Contains("openwrt", StringComparison.OrdinalIgnoreCase) ?? false));

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Router &&
        ((vendor?.Contains("openwrt", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (tagsCsv?.Contains("openwrt", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (hostname?.Contains("openwrt", StringComparison.OrdinalIgnoreCase) ?? false));

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "OpenWrt Router" : name,
            Category: HardwareCategory.Router,
            IpAddress: ipAddress ?? "192.168.1.1",
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 4,
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"http://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "router, openwrt",
            Vendor: "OpenWrt",
            Model: "OpenWrt Router / Appliance");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Router,
            PortCount = current.PortCount <= 0 ? 4 : current.PortCount,
            Vendor = string.IsNullOrWhiteSpace(current.Vendor) ? "OpenWrt" : current.Vendor
        };

    public IEnumerable<NetAdmin.Core.Models.DeviceCapability> GetCapabilities(HardwareDevice device)
    {
        var caps = new List<NetAdmin.Core.Models.DeviceCapability>();
        if (!string.IsNullOrWhiteSpace(device.IpAddress)) caps.Add(NetAdmin.Core.Models.DeviceCapability.WebGui);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Probe);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Ssh);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Reboot);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.VlanConfig);
        return caps;
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("ubus:username", "LuCI / ubus RPC Username", "root"),
        new("ubus:password", "LuCI / ubus RPC Password", "RPC password", IsSecret: true),
        new("ssh:username", "SSH Username", "root"),
        new("ssh:password", "SSH Password / Key", "SSH password", IsSecret: true)
    ];
}
