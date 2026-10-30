using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.Shelly.Components;

namespace NetAdmin.Providers.Shelly;

public class ShellyHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "shelly";
    public string DisplayName => "⚡ Shelly IoT";
    public string Icon => "⚡";
    public HardwareCategory Category => HardwareCategory.IoT;
    public int Priority => 100;

    public Type? EditorComponentType => typeof(ShellyEditor);
    public Type? HeaderActionsComponentType => typeof(ShellyHeaderActions);
    public Type? CardSummaryComponentType => typeof(ShellyCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is ShellyHardwareDevice ||
        (device.Category == HardwareCategory.IoT &&
         (device.Vendor.Equals("Shelly", StringComparison.OrdinalIgnoreCase) ||
          device.TagsCsv.Contains("shelly", StringComparison.OrdinalIgnoreCase)));

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.IoT &&
        ((vendor?.Equals("Shelly", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (tagsCsv?.Contains("shelly", StringComparison.OrdinalIgnoreCase) ?? false) ||
         (hostname?.Contains("shelly", StringComparison.OrdinalIgnoreCase) ?? false));

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new ShellyHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Shelly Device" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            ShellyModel: "Plus 1PM",
            Generation: 2,
            Notes: "",
            TagsCsv: "shelly, iot");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is ShellyHardwareDevice sh)
        {
            return sh with { Category = HardwareCategory.IoT, Vendor = "Shelly", Model = sh.ShellyModel };
        }

        return new ShellyHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            ShellyModel: string.IsNullOrWhiteSpace(current.Model) ? "Plus 1PM" : current.Model,
            Generation: 2,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "shelly, iot" : current.TagsCsv)
        {
            PortCount = current.PortCount,
            ManagementUrl = current.ManagementUrl
        };
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("http:username", "HTTP Auth Username (Gen1 Optional)", "admin"),
        new("http:password", "Shelly RPC / HTTP Password", "Leave empty if unprotected", IsSecret: true)
    ];
}
