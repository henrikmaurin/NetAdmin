using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.UniFi.Components;

namespace NetAdmin.Providers.UniFi;

public class AccessPointHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "accesspoint";
    public string DisplayName => "📶 Access Point";
    public string Icon => "📶";
    public HardwareCategory Category => HardwareCategory.AccessPoint;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(AccessPointEditor);
    public Type? HeaderActionsComponentType => typeof(AccessPointHeaderActions);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.AccessPoint;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.AccessPoint;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Access Point" : name,
            Category: HardwareCategory.AccessPoint,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "ap, wifi, unifi",
            Vendor: "Ubiquiti UniFi",
            Model: "U6-Pro");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.AccessPoint,
            PortCount = current.PortCount <= 0 ? 1 : current.PortCount
        };

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device)
    {
        var isUniFi = (device.Vendor?.Contains("UniFi", StringComparison.OrdinalIgnoreCase) ?? false) ||
                      (device.Vendor?.Contains("Ubiquiti", StringComparison.OrdinalIgnoreCase) ?? false) ||
                      (device.TagsCsv?.Contains("unifi", StringComparison.OrdinalIgnoreCase) ?? false);

        if (isUniFi)
        {
            return
            [
                new("unifi:baseurl", "UniFi Controller Base URL", "https://192.168.1.10:8443", IsSecret: false, GlobalSettingKey: "UniFi:BaseUrl", HelpText: "Synced with UniFi Controller settings"),
                new("unifi:site", "UniFi Site Name", "default", IsSecret: false, GlobalSettingKey: "UniFi:Site"),
                new("unifi:username", "UniFi Controller Username", "svc_netadmin", IsSecret: false, GlobalSettingKey: "UniFi:Username"),
                new("unifi:password", "UniFi Controller Password", "Controller password", IsSecret: true, GlobalSettingKey: "UniFi:Password"),
                new("unifi:apikey", "UniFi API Key (Network 8.5+ Optional)", "X-API-KEY", IsSecret: true, GlobalSettingKey: "UniFi:ApiKey"),
                new("ssh:username", "Device SSH Username", "ubnt"),
                new("ssh:password", "Device SSH Password", "SSH password", IsSecret: true)
            ];
        }

        return
        [
            new("http:username", "Web / API Username", "admin"),
            new("http:password", "Web / API Password", "Password", IsSecret: true),
            new("ssh:username", "SSH Username", "admin"),
            new("ssh:password", "SSH Password", "SSH password", IsSecret: true)
        ];
    }
}
