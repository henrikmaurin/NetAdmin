namespace NetAdmin.Providers.Shelly.Products;

public sealed class Shelly1Gen1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1-gen1";
    public override string Model => "1 (Gen1)";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 1;
    public override IReadOnlyList<string> DeviceCodes => ["SHSW-1", "Shelly1"];
    public override string DefaultTagsCsv => "shelly, iot, relay, gen1";
    public override string Description => "1-kanals 16A potentialfritt relä (Wi-Fi)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1PmGen1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1pm-gen1";
    public override string Model => "1PM (Gen1)";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 1;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SHSW-PM", "Shelly1PM"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, gen1";
    public override string Description => "1-kanals 16A relä med effektmätning (Wi-Fi)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly25Gen1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-25-gen1";
    public override string Model => "2.5 (Gen1)";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 1;
    public override int RelayChannels => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SHSW-25", "Shelly25"];
    public override string DefaultTagsCsv => "shelly, iot, relay, shutter, pm, gen1";
    public override string Description => "2-kanals relä & jalusikontroller med effektmätning";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus1Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-1";
    public override string Model => "Plus 1";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 2;
    public override IReadOnlyList<string> DeviceCodes => ["SNSW-001X16EU", "Plus1"];
    public override string DefaultTagsCsv => "shelly, iot, relay, gen2";
    public override string Description => "1-kanals 16A potentialfritt relä (Wi-Fi + Bluetooth)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus1MiniProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-1-mini";
    public override string Model => "Plus 1 Mini";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 2;
    public override IReadOnlyList<string> DeviceCodes => ["SNSW-001X8EU", "Plus1Mini"];
    public override string DefaultTagsCsv => "shelly, iot, relay, mini, gen2";
    public override string Description => "Kompakt 1-kanals 8A relä (Wi-Fi + Bluetooth)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus1PmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-1pm";
    public override string Model => "Plus 1PM";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SNSW-001P16EU", "Plus1PM"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, gen2";
    public override string Description => "1-kanals 16A relä med effektmätning (Wi-Fi + Bluetooth)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus1PmMiniProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-1pm-mini";
    public override string Model => "Plus 1PM Mini";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SNSW-001P8EU", "Plus1PMMini"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, mini, gen2";
    public override string Description => "Kompakt 1-kanals 8A relä med effektmätning";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus2PmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-2pm";
    public override string Model => "Plus 2PM";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 2;
    public override int RelayChannels => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SNSW-002P16EU", "SNSW-102P16EU", "Plus2PM"];
    public override string DefaultTagsCsv => "shelly, iot, relay, shutter, pm, gen2";
    public override string Description => "2-kanals relä / jalusikontroller med effektmätning";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1Gen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1-gen3";
    public override string Model => "1 Gen3";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 3;
    public override IReadOnlyList<string> DeviceCodes => ["S3SW-001X16EU", "S1G3"];
    public override string DefaultTagsCsv => "shelly, iot, relay, gen3";
    public override string Description => "1-kanals 16A relä med Shelly-chip & 8MB minne (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1PmGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1pm-gen3";
    public override string Model => "1PM Gen3";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 3;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3SW-001P16EU", "S1PMG3"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, gen3";
    public override string Description => "1-kanals 16A relä med effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1MiniGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1-mini-gen3";
    public override string Model => "1 Mini Gen3";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 3;
    public override IReadOnlyList<string> DeviceCodes => ["S3SW-001X8EU", "Mini1G3"];
    public override string DefaultTagsCsv => "shelly, iot, relay, mini, gen3";
    public override string Description => "Kompakt 8A relä (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1PmMiniGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1pm-mini-gen3";
    public override string Model => "1PM Mini Gen3";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 3;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3SW-001P8EU", "Mini1PMG3"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, mini, gen3";
    public override string Description => "Kompakt 8A relä med effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly2PmGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-2pm-gen3";
    public override string Model => "2PM Gen3";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 3;
    public override int RelayChannels => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3SW-002P16EU", "S2PMG3"];
    public override string DefaultTagsCsv => "shelly, iot, relay, shutter, pm, gen3";
    public override string Description => "2-kanals 16A relä med effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1Gen4Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1-gen4";
    public override string Model => "1 Gen4";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 4;
    public override IReadOnlyList<string> DeviceCodes => ["S4SW-001X16EU", "S1G4"];
    public override string DefaultTagsCsv => "shelly, iot, relay, matter, zigbee, gen4";
    public override string Description => "Multi-protokoll (Wi-Fi, Matter, Zigbee, BLE) 16A relä (Gen4)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class Shelly1PmGen4Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-1pm-gen4";
    public override string Model => "1PM Gen4";
    public override string SubCategory => "Smart Switches & Relays";
    public override int Generation => 4;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S4SW-001P16EU", "S1PMG4"];
    public override string DefaultTagsCsv => "shelly, iot, relay, pm, matter, zigbee, gen4";
    public override string Description => "Multi-protokoll (Wi-Fi, Matter, Zigbee, BLE) 16A relä med effektmätning (Gen4)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}
