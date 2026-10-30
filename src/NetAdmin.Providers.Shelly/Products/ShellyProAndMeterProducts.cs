using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Shelly.Products;

public sealed class ShellyPro1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-1";
    public override string Model => "Pro 1";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-001XE16EU", "SPSW-201XE16EU", "Pro1"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, gen2";
    public override string Description => "DIN-skenemonterat 1-kanals 16A relä med RJ45 LAN, Wi-Fi & BLE";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro1PmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-1pm";
    public override string Model => "Pro 1PM";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-001PE16EU", "SPSW-201PE16EU", "Pro1PM"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, pm, gen2";
    public override string Description => "DIN-skenemonterat 1-kanals 16A relä med effektmätning & RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro2Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-2";
    public override string Model => "Pro 2";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 2;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-002XE16EU", "SPSW-202XE16EU", "Pro2"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, gen2";
    public override string Description => "DIN-skenemonterat 2-kanals relä med RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro2PmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-2pm";
    public override string Model => "Pro 2PM";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 2;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-002PE16EU", "SPSW-202PE16EU", "Pro2PM"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, pm, gen2";
    public override string Description => "DIN-skenemonterat 2-kanals relä med effektmätning & RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-3";
    public override string Model => "Pro 3";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 3;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-003XE16EU", "Pro3"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, gen2";
    public override string Description => "DIN-skenemonterat 3-kanals 16A relä med RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro4PmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-4pm";
    public override string Model => "Pro 4PM";
    public override string SubCategory => "Professional DIN (LAN)";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 4;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPSW-004PE16EU", "SPSW-104PE16EU", "Pro4PM"];
    public override string DefaultTagsCsv => "shelly, iot, pro, din, lan, relay, pm, gen2";
    public override string Description => "DIN-skenemonterat 4-kanals 16A relä med färgdisplay, effektmätning & RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/professional-devices";
}

public sealed class ShellyPro3EmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-3em";
    public override string Model => "Pro 3EM";
    public override HardwareCategory Category => HardwareCategory.EnergyMeter;
    public override string SubCategory => "Energy Meters";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 0;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPEM-003CEBEU", "SPEM-003CEBEU120", "SPEM-003CEBEU400", "Pro3EM"];
    public override string DefaultTagsCsv => "shelly, energymeter, pro, din, lan, 3-phase, gen2";
    public override string Description => "DIN-skenemonterad 3-fas energimätare med RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/energy-meters";
}

public sealed class ShellyProEm50Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-pro-em-50";
    public override string Model => "Pro EM-50";
    public override HardwareCategory Category => HardwareCategory.EnergyMeter;
    public override string SubCategory => "Energy Meters";
    public override int Generation => 2;
    public override int DefaultPortCount => 1;
    public override int RelayChannels => 1;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SPEM-002CEBEU50", "ProEM", "ProEM50"];
    public override string DefaultTagsCsv => "shelly, energymeter, pro, din, lan, gen2";
    public override string Description => "DIN-skenemonterad 1-fas 2-kanals energimätare med RJ45 LAN";
    public override string ProductUrl => "https://www.shelly.com/collections/energy-meters";
}

public sealed class ShellyEmGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-em-gen3";
    public override string Model => "EM Gen3";
    public override HardwareCategory Category => HardwareCategory.EnergyMeter;
    public override string SubCategory => "Energy Meters";
    public override int Generation => 3;
    public override int RelayChannels => 1;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3EM-002CXCEU", "EMG3"];
    public override string DefaultTagsCsv => "shelly, energymeter, gen3";
    public override string Description => "Kompakt energimätare med kontaktorkontroll (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/energy-meters";
}

public sealed class Shelly3EmGen1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-3em-gen1";
    public override string Model => "3EM (Gen1)";
    public override HardwareCategory Category => HardwareCategory.EnergyMeter;
    public override string SubCategory => "Energy Meters";
    public override int Generation => 1;
    public override int RelayChannels => 1;
    public override bool HasPowerMetering => true;
    public override bool IsDinRailMountable => true;
    public override IReadOnlyList<string> DeviceCodes => ["SHEM-3", "Shelly3EM"];
    public override string DefaultTagsCsv => "shelly, energymeter, 3-phase, gen1";
    public override string Description => "3-fas energimätare för DIN-skena (Gen1 Wi-Fi)";
    public override string ProductUrl => "https://www.shelly.com/collections/energy-meters";
}
