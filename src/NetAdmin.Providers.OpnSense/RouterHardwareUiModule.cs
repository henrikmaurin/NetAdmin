using NetAdmin.Core.Components;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.OpnSense.Components;

namespace NetAdmin.Providers.OpnSense;

public class RouterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "router";
    public string DisplayName => "🔥 Router";
    public string Icon => "🔥";
    public HardwareCategory Category => HardwareCategory.Router;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(CommonRouterEditor);
    public Type? HeaderActionsComponentType => typeof(RouterHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Router;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Router;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Router" : name,
            Category: HardwareCategory.Router,
            IpAddress: ipAddress ?? "192.168.1.1",
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 4,
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"https://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "router, firewall",
            Vendor: "OPNsense",
            Model: "Firewall / Router");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Router,
            PortCount = current.PortCount <= 0 ? 4 : current.PortCount
        };

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device)
    {
        var isOpnSense = (device.Vendor?.Contains("OPNsense", StringComparison.OrdinalIgnoreCase) ?? false) ||
                         (device.TagsCsv?.Contains("opnsense", StringComparison.OrdinalIgnoreCase) ?? false) ||
                         (device.Id?.Contains("opnsense", StringComparison.OrdinalIgnoreCase) ?? false);

        if (isOpnSense)
        {
            return
            [
                new("api:baseurl", "OPNsense API Base URL", "https://192.168.1.1", IsSecret: false, GlobalSettingKey: "OpnSense:BaseUrl", HelpText: "Synced with OPNsense integration settings"),
                new("api:key", "OPNsense API Key", "Paste key=...", IsSecret: true, GlobalSettingKey: "OpnSense:ApiKey"),
                new("api:secret", "OPNsense API Secret", "Paste secret=...", IsSecret: true, GlobalSettingKey: "OpnSense:ApiSecret"),
                new("ssh:username", "SSH Username", "root"),
                new("ssh:password", "SSH Password / Key", "SSH password or key", IsSecret: true)
            ];
        }

        return
        [
            new("http:username", "Admin / API Username", "admin"),
            new("http:password", "Admin / API Password", "Password", IsSecret: true),
            new("http:token", "HTTP Bearer Token / API Key", "Optional API token", IsSecret: true),
            new("ssh:username", "SSH Username", "root"),
            new("ssh:password", "SSH Password / Key", "SSH password or key", IsSecret: true)
        ];
    }
}
