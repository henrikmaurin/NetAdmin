using NetAdmin.Core.Models;

namespace NetAdmin.Providers.HomeWizard.Products;

/// <summary>
/// Abstrakt basklass för alla dedikerade energimätare (HomeWizard, HAN/P1-mätare, DIN-mätare).
/// </summary>
public abstract class EnergyMeterProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.EnergyMeter;

    /// <summary>Typ av mätgränssnitt (t.ex. "RJ12 P1 / HAN", "MID 3-Fas DIN", "MID 1-Fas DIN", "3-Fas CT").</summary>
    public abstract string MeterInterface { get; }

    public override HardwareDevice ApplyToDevice(HardwareDevice device)
    {
        if (device is EnergyMeterHardwareDevice meter)
        {
            return meter with
            {
                Category = HardwareCategory.EnergyMeter,
                Vendor = Vendor,
                MeterModel = Model,
                Model = Model,
                PortCount = DefaultPortCount,
                MeterInterface = MeterInterface,
                TagsCsv = DefaultTagsCsv
            };
        }

        return new EnergyMeterHardwareDevice(
            Id: device.Id,
            Name: device.Name,
            IpAddress: device.IpAddress,
            MacAddress: device.MacAddress,
            Room: device.Room,
            Vendor: Vendor,
            MeterModel: Model,
            PortCount: DefaultPortCount,
            MeterInterface: MeterInterface,
            ManagementUrl: device.ManagementUrl,
            Notes: device.Notes,
            TagsCsv: DefaultTagsCsv);
    }
}

public sealed class HomeWizardP1MeterProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "homewizard-wi-fi-p1-meter";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi P1 Meter (HWE-P1)";
    public override string SubCategory => "P1 / HAN Smart Meters";
    public override int DefaultPortCount => 0;
    public override string MeterInterface => "RJ12 P1 / HAN (DSMR), Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["HWE-P1", "P1 Meter", "hw-p1meter", "p1meter"];
    public override string DefaultTagsCsv => "energymeter, homewizard, p1-meter, han, rj12, wifi";
    public override string Description => "Real-time smart electricity meter reader for RJ12 P1/HAN port with local REST API (/api/v1/data) and Wi-Fi";
    public override string ProductUrl => "https://www.homewizard.com/p1-meter/";
}

public sealed class HomeWizardKwhMeter3PhaseProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "homewizard-kwh-meter-3phase";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi kWh Meter 3-Phase (SDM630)";
    public override string SubCategory => "DIN Energy Meters";
    public override int DefaultPortCount => 0;
    public override string MeterInterface => "MID 3-Phase DIN (100A), Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["HWE-KWH3", "SDM630-wifi", "kWh Meter 3-Phase"];
    public override string DefaultTagsCsv => "energymeter, homewizard, 3-phase, din, mid, wifi";
    public override string Description => "MID-approved 3-phase DIN rail energy meter with Wi-Fi (for EV chargers, solar inverters, heat pumps)";
}

public sealed class HomeWizardKwhMeter1PhaseProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "homewizard-kwh-meter-1phase";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi kWh Meter 1-Phase (SDM230)";
    public override string SubCategory => "DIN Energy Meters";
    public override int DefaultPortCount => 0;
    public override string MeterInterface => "MID 1-Phase DIN (100A), Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["HWE-KWH1", "SDM230-wifi", "kWh Meter 1-Phase"];
    public override string DefaultTagsCsv => "energymeter, homewizard, 1-phase, din, mid, wifi";
    public override string Description => "MID-approved 1-phase DIN rail energy meter with Wi-Fi";
}

public sealed class HomeWizardWatermeterProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "homewizard-wi-fi-watermeter";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi Watermeter (HWE-WTR)";
    public override string SubCategory => "Water & Utility Meters";
    public override int DefaultPortCount => 0;
    public override string MeterInterface => "Optical / Inductive water meter sensor, Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["HWE-WTR", "Watermeter"];
    public override string DefaultTagsCsv => "energymeter, homewizard, watermeter, wifi";
    public override string Description => "Wireless real-time water consumption meter with local REST API";
}

public sealed class HomeWizardEnergySocketProduct : HardwareProductDefinition
{
    public override string ProductId => "homewizard-energy-socket";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi Energy Socket (HWE-SKT)";
    public override HardwareCategory Category => HardwareCategory.IoT;
    public override string SubCategory => "Smart Plugs";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["HWE-SKT", "Energy Socket"];
    public override string DefaultTagsCsv => "iot, homewizard, plug, energy-meter, wifi";
    public override string Description => "Smart 16A wall socket with precision energy metering and local REST API (Wi-Fi)";
    public override string ProductUrl => "https://www.homewizard.com/energy-socket/";
}

public sealed class HomeWizardP1MeterIoTProduct : HardwareProductDefinition
{
    public override string ProductId => "homewizard-p1-meter-iot";
    public override string Vendor => "HomeWizard";
    public override string Model => "Wi-Fi P1 Meter (HWE-P1)";
    public override HardwareCategory Category => HardwareCategory.IoT;
    public override string SubCategory => "P1 / HAN Smart Meters";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["HWE-P1", "P1 Meter", "hw-p1meter", "p1meter"];
    public override string DefaultTagsCsv => "iot, energymeter, homewizard, p1-meter, han, rj12, wifi";
    public override string Description => "HomeWizard Wi-Fi P1 Meter for RJ12 P1/HAN smart electricity meter port";
    public override string ProductUrl => "https://www.homewizard.com/p1-meter/";
}

public sealed class GenericP1HanEnergyMeterProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "generic-energymeter-p1-han";
    public override string Vendor => "Generic";
    public override string Model => "P1 / HAN Energy Meter (Wi-Fi)";
    public override string SubCategory => "Generic Energy Meter";
    public override int DefaultPortCount => 0;
    public override string MeterInterface => "RJ12 P1 / RJ45 HAN, Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["GenericP1Meter", "TibberPulse", "EaseeEqualizer"];
    public override string DefaultTagsCsv => "energymeter, p1-meter, han, wifi";
    public override string Description => "Generic smart electricity meter reader for HAN/P1 port";
}

public sealed class GenericDinLanEnergyMeterProduct : EnergyMeterProductDefinition
{
    public override string ProductId => "generic-energymeter-din-lan";
    public override string Vendor => "Generic";
    public override string Model => "3-Phase DIN Energy Meter (LAN)";
    public override string SubCategory => "Generic Energy Meter";
    public override int DefaultPortCount => 1;
    public override string MeterInterface => "3-Phase DIN / Modbus TCP (RJ45 LAN)";
    public override IReadOnlyList<string> DeviceCodes => ["GenericDinMeter"];
    public override string DefaultTagsCsv => "energymeter, 3-phase, din, lan";
    public override string Description => "Generic DIN rail 3-phase energy meter with physical RJ45 Ethernet port";
}
