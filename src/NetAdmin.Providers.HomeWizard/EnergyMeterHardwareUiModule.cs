using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.HomeWizard.Components;

namespace NetAdmin.Providers.HomeWizard;

public class EnergyMeterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "energymeter";
    public string DisplayName => "📊 Energy Meter";
    public string Icon => "📊";
    public HardwareCategory Category => HardwareCategory.EnergyMeter;
    public int Priority => 85;

    public Type? EditorComponentType => typeof(EnergyMeterEditor);
    public Type? CardSummaryComponentType => typeof(EnergyMeterCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is EnergyMeterHardwareDevice || device.Category == HardwareCategory.EnergyMeter;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.EnergyMeter ||
        (vendor?.Equals("HomeWizard", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("p1meter", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("homewizard", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("kwhmeter", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new EnergyMeterHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Energy Meter" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Utility Cabinet" : room,
            Vendor: "HomeWizard",
            MeterModel: "Wi-Fi P1 Meter (HWE-P1)",
            PortCount: 0,
            MeterInterface: "RJ12 P1 / HAN (DSMR), Wi-Fi",
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "energymeter, homewizard, p1-meter, han, rj12, wifi");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is EnergyMeterHardwareDevice meter)
        {
            return meter with { Category = HardwareCategory.EnergyMeter, Model = meter.MeterModel };
        }

        return new EnergyMeterHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            Vendor: string.IsNullOrWhiteSpace(current.Vendor) ? "HomeWizard" : current.Vendor,
            MeterModel: string.IsNullOrWhiteSpace(current.Model) ? "Wi-Fi P1 Meter (HWE-P1)" : current.Model,
            PortCount: current.PortCount,
            MeterInterface: "RJ12 P1 / HAN (DSMR), Wi-Fi",
            ManagementUrl: current.ManagementUrl,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "energymeter, homewizard, p1-meter, han" : current.TagsCsv);
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("http:token", "Local API Token (Optional, v2 API)", "Bearer token if required", IsSecret: true)
    ];
}
