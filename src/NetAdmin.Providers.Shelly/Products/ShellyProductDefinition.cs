using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Shelly.Products;

/// <summary>
/// Abstrakt basklass för alla Shelly-produkter.
/// Varje konkret Shelly-produkt har sin egen klass som ärver härifrån.
/// </summary>
public abstract class ShellyProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "Shelly";
    public override HardwareCategory Category => HardwareCategory.IoT;

    public abstract override int Generation { get; }

    /// <summary>Antal relä-/switchkanaler på enheten.</summary>
    public virtual int RelayChannels => 1;

    /// <summary>Har enheten inbyggd effektmätning (Power Metering)?</summary>
    public virtual bool HasPowerMetering => false;

    /// <summary>Stödjer enheten DIN-skenemontage?</summary>
    public virtual bool IsDinRailMountable => false;

    public override HardwareDevice ApplyToDevice(HardwareDevice device)
    {
        if (device is ShellyHardwareDevice sh)
        {
            return sh with
            {
                Category = HardwareCategory.IoT,
                Vendor = Vendor,
                ShellyModel = Model,
                Model = Model,
                Generation = Generation,
                PortCount = DefaultPortCount,
                TagsCsv = string.IsNullOrWhiteSpace(sh.TagsCsv) ? DefaultTagsCsv : sh.TagsCsv
            };
        }

        return new ShellyHardwareDevice(
            Id: device.Id,
            Name: device.Name,
            IpAddress: device.IpAddress,
            MacAddress: device.MacAddress,
            Room: device.Room,
            ShellyModel: Model,
            Generation: Generation,
            Notes: device.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(device.TagsCsv) ? DefaultTagsCsv : device.TagsCsv)
        {
            PortCount = DefaultPortCount,
            ManagementUrl = device.ManagementUrl
        };
    }
}

public sealed class GenericShellyWifiProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-generic-wifi";
    public override string Model => "Generic Shelly (Wi-Fi)";
    public override string SubCategory => "Generic Shelly";
    public override int Generation => 2;
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["ShellyGeneric"];
    public override string DefaultTagsCsv => "shelly, iot, wifi";
    public override string Description => "Generic Wi-Fi connected Shelly IoT device";
}

public sealed class GenericShellyProLanProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-generic-pro-lan";
    public override string Model => "Generic Shelly Pro (LAN)";
    public override string SubCategory => "Generic Shelly";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["ShellyProGeneric"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan";
    public override string Description => "Generic DIN-rail mounted Shelly Pro device with physical RJ45 LAN port";
}
