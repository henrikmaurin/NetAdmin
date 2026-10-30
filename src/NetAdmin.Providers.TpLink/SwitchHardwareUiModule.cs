using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.TpLink.Components;

namespace NetAdmin.Providers.TpLink;

public class SwitchHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "switch";
    public string DisplayName => "🖧 Switch";
    public string Icon => "🖧";
    public HardwareCategory Category => HardwareCategory.Switch;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(SwitchEditor);
    public Type? HeaderActionsComponentType => typeof(SwitchHeaderActions);
    public Type? CardSummaryComponentType => typeof(SwitchCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is SwitchHardwareDevice || device.Category == HardwareCategory.Switch;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Switch;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new SwitchHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Switch" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 8,
            IsManaged: !string.IsNullOrWhiteSpace(ipAddress),
            Vendor: "TP-Link",
            SwitchModel: "TL-SG108PE",
            ManagementUrl: !string.IsNullOrWhiteSpace(ipAddress) ? $"http://{ipAddress}" : null,
            Notes: "",
            TagsCsv: "switch");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is SwitchHardwareDevice sw)
        {
            return sw with { Category = HardwareCategory.Switch, Model = sw.SwitchModel };
        }

        return new SwitchHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            PortCount: current.PortCount <= 1 ? 8 : current.PortCount,
            IsManaged: !string.IsNullOrWhiteSpace(current.IpAddress),
            Vendor: string.IsNullOrWhiteSpace(current.Vendor) ? "Generic" : current.Vendor,
            SwitchModel: string.IsNullOrWhiteSpace(current.Model) ? "8-Port Switch" : current.Model,
            ManagementUrl: current.ManagementUrl,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "switch" : current.TagsCsv);
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device)
    {
        if (device is SwitchHardwareDevice { IsManaged: false })
            return [];

        return
        [
            new("http:username", "Switch Web / API Username", "admin"),
            new("http:password", "Switch Web / API Password", "Switch password", IsSecret: true),
            new("ssh:username", "SSH / CLI Username (Optional)", "admin"),
            new("ssh:password", "SSH / CLI Password (Optional)", "SSH password", IsSecret: true)
        ];
    }
}
