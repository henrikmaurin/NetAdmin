using NetAdmin.Core.Components;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.PfSense.Components;

namespace NetAdmin.Providers.PfSense;

public class PfSenseRouterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "pfsense-router";
    public string DisplayName => "🛡️ pfSense Router";
    public string Icon => "🛡️";
    public HardwareCategory Category => HardwareCategory.Router;
    public int Priority => 60;

    public Type? EditorComponentType => typeof(CommonRouterEditor);
    public Type? HeaderActionsComponentType => typeof(PfSenseHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Router &&
        ((device.Vendor?.Contains("pfsense", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (device.Vendor?.Contains("netgate", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (device.TagsCsv?.Contains("pfsense", StringComparison.OrdinalIgnoreCase) ?? false));

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Router &&
        ((vendor?.Contains("pfsense", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (vendor?.Contains("netgate", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (tagsCsv?.Contains("pfsense", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (hostname?.Contains("pfsense", StringComparison.OrdinalIgnoreCase) ?? false));

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "pfSense Router" : name,
            Category: HardwareCategory.Router,
            IpAddress: ipAddress ?? "192.168.1.1",
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 4,
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"https://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "router, firewall, pfsense",
            Vendor: "pfSense",
            Model: "pfSense CE / Plus Firewall Appliance");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Router,
            PortCount = current.PortCount <= 0 ? 4 : current.PortCount,
            Vendor = string.IsNullOrWhiteSpace(current.Vendor) ? "pfSense" : current.Vendor
        };

    public IEnumerable<NetAdmin.Core.Models.DeviceCapability> GetCapabilities(HardwareDevice device)
    {
        // Similar conservative defaults
        var caps = new List<NetAdmin.Core.Models.DeviceCapability>();
        if (!string.IsNullOrWhiteSpace(device.IpAddress)) caps.Add(NetAdmin.Core.Models.DeviceCapability.WebGui);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Probe);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.FirewallConfig);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.StaticDhcp);
        caps.Add(NetAdmin.Core.Models.DeviceCapability.Reboot);
        return caps;
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("api:key", "pfSense API Key / Client ID", "Client ID or API Key", IsSecret: true),
        new("api:secret", "pfSense API Token / Client Secret", "Client Token", IsSecret: true),
        new("ssh:username", "SSH Username", "admin"),
        new("ssh:password", "SSH Password / Key", "SSH password", IsSecret: true)
    ];
}
