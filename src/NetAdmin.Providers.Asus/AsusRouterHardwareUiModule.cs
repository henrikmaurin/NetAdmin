using NetAdmin.Core.Components;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.Asus;

public class AsusRouterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "asus-router";
    public string DisplayName => "🔧 ASUS Router";
    public string Icon => "🔧";
    public HardwareCategory Category => HardwareCategory.Router;
    public int Priority => 60; // slightly higher to prefer over generic router module

    public Type? EditorComponentType => typeof(CommonRouterEditor);
    public Type? HeaderActionsComponentType => typeof(Components.AsusHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Router &&
        ((device.Vendor?.Contains("ASUS", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (device.TagsCsv?.Contains("asus", StringComparison.OrdinalIgnoreCase) ?? false));

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Router &&
        ((vendor?.Contains("ASUS", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (tagsCsv?.Contains("asus", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (hostname?.Contains("asus", StringComparison.OrdinalIgnoreCase) ?? false));

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "ASUS Router" : name,
            Category: HardwareCategory.Router,
            IpAddress: ipAddress ?? "192.168.1.1",
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 4,
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"https://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "router, asus",
            Vendor: "ASUS",
            Model: "ASUS Router");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Router,
            PortCount = current.PortCount <= 0 ? 4 : current.PortCount
        };

    public IEnumerable<NetAdmin.Core.Models.DeviceCapability> GetCapabilities(HardwareDevice device)
    {
        var caps = new List<NetAdmin.Core.Models.DeviceCapability>();
        if (!string.IsNullOrWhiteSpace(device.EffectiveManagementUrl) || !string.IsNullOrWhiteSpace(device.ManagementUrl) || !string.IsNullOrWhiteSpace(device.IpAddress))
            caps.Add(NetAdmin.Core.Models.DeviceCapability.WebGui);

        // We can't be sure without probing; expose Probe capability and optimistically SSH when tags hint it
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Probe);
        if ((device.TagsCsv ?? "").Contains("ssh", StringComparison.OrdinalIgnoreCase))
            caps.Add(NetAdmin.Core.Models.DeviceCapability.Ssh);

        caps.Add(NetAdmin.Core.Models.DeviceCapability.Reboot);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.VlanConfig);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.FirewallConfig);
        return caps;
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("ssh:username", "SSH Username", "admin"),
        new("ssh:password", "SSH Password / Key", "SSH password", IsSecret: true),
        new("http:username", "Web GUI Username", "admin"),
        new("http:password", "Web GUI Password", "Web password", IsSecret: true)
    ];
}
