namespace NetAdmin.Providers.Shelly.Products;

public sealed class ShellyPlusPlugSProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-plug-s";
    public override string Model => "Plus Plug S";
    public override string SubCategory => "Smart Plugs";
    public override int Generation => 2;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SNPL-00112EU", "PlusPlugS"];
    public override string DefaultTagsCsv => "shelly, iot, plug, pm, gen2";
    public override string Description => "Smart vägguttag 12A med LED-indikator och effektmätning (Gen2)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-plugs";
}

public sealed class ShellyPlugSGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-plug-s-gen3";
    public override string Model => "Plug S MTR Gen3";
    public override string SubCategory => "Smart Plugs";
    public override int Generation => 3;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3PL-00112EU", "PlugSG3"];
    public override string DefaultTagsCsv => "shelly, iot, plug, pm, matter, gen3";
    public override string Description => "Smart vägguttag med Matter-stöd och effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-plugs";
}

public sealed class ShellyOutdoorPlugSGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-outdoor-plug-s-gen3";
    public override string Model => "Outdoor Plug S Gen3";
    public override string SubCategory => "Smart Plugs";
    public override int Generation => 3;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3PL-20112EU", "OutdoorPlugSG3"];
    public override string DefaultTagsCsv => "shelly, iot, plug, outdoor, pm, matter, gen3";
    public override string Description => "IP44-klassat utomhusuttag med Matter och effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-plugs";
}

public sealed class ShellyDimmer2Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-dimmer-2";
    public override string Model => "Dimmer 2 (Gen1)";
    public override string SubCategory => "Dimmers & Lighting";
    public override int Generation => 1;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SHDM-2", "ShellyDimmer2"];
    public override string DefaultTagsCsv => "shelly, iot, dimmer, pm, gen1";
    public override string Description => "Smart dimmer som fungerar med och utan nolla (Gen1)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlus010vDimmerProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-0-10v-dimmer";
    public override string Model => "Plus 0-10V Dimmer";
    public override string SubCategory => "Dimmers & Lighting";
    public override int Generation => 2;
    public override IReadOnlyList<string> DeviceCodes => ["SNDM-00100WW", "Plus010VDimmer"];
    public override string DefaultTagsCsv => "shelly, iot, dimmer, 0-10v, gen2";
    public override string Description => "Smart 0-10V dimmerkontroller (Gen2)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyDimmer0110vPmGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-dimmer-0-1-10v-pm-gen3";
    public override string Model => "Dimmer 0/1-10V PM Gen3";
    public override string SubCategory => "Dimmers & Lighting";
    public override int Generation => 3;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["S3DM-0010WW", "Dimmer0110VPMG3"];
    public override string DefaultTagsCsv => "shelly, iot, dimmer, pm, gen3";
    public override string Description => "Smart 0/1-10V dimmer med inbyggd effektmätning (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-switches-and-dimmers";
}

public sealed class ShellyPlusRgbwPmProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-rgbw-pm";
    public override string Model => "Plus RGBW PM";
    public override string SubCategory => "Dimmers & Lighting";
    public override int Generation => 2;
    public override int RelayChannels => 4;
    public override bool HasPowerMetering => true;
    public override IReadOnlyList<string> DeviceCodes => ["SNDC-0D4P10WW", "PlusRGBWPM"];
    public override string DefaultTagsCsv => "shelly, iot, rgbw, led, pm, gen2";
    public override string Description => "12/24V DC RGBW LED-kontroller med effektmätning (Gen2)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-controllers";
}

public sealed class ShellyPlusI4Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-i4";
    public override string Model => "Plus i4";
    public override string SubCategory => "Sensors & Controllers";
    public override int Generation => 2;
    public override int RelayChannels => 0;
    public override IReadOnlyList<string> DeviceCodes => ["SNSN-0024X", "PlusI4"];
    public override string DefaultTagsCsv => "shelly, iot, controller, i4, gen2";
    public override string Description => "4-kanals digital scen- och knappkontroller utan relä (Gen2)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-controllers";
}

public sealed class ShellyI4Gen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-i4-gen3";
    public override string Model => "i4 Gen3";
    public override string SubCategory => "Sensors & Controllers";
    public override int Generation => 3;
    public override int RelayChannels => 0;
    public override IReadOnlyList<string> DeviceCodes => ["S3SN-0024X", "I4G3"];
    public override string DefaultTagsCsv => "shelly, iot, controller, i4, gen3";
    public override string Description => "4-kanals scen- och knappkontroller (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-controllers";
}

public sealed class ShellyPlusHtProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-plus-ht";
    public override string Model => "Plus H&T";
    public override string SubCategory => "Sensors & Controllers";
    public override int Generation => 2;
    public override int RelayChannels => 0;
    public override IReadOnlyList<string> DeviceCodes => ["SNSN-0013A", "PlusHT"];
    public override string DefaultTagsCsv => "shelly, iot, sensor, temperature, humidity, gen2";
    public override string Description => "Trådlös temperatur- och luftfuktighetssensor med e-ink-skärm (Gen2)";
    public override string ProductUrl => "https://www.shelly.com/collections/sensors-and-thermostats";
}

public sealed class ShellyHtGen3Product : ShellyProductDefinition
{
    public override string ProductId => "shelly-ht-gen3";
    public override string Model => "H&T Gen3";
    public override string SubCategory => "Sensors & Controllers";
    public override int Generation => 3;
    public override int RelayChannels => 0;
    public override IReadOnlyList<string> DeviceCodes => ["S3SN-0U12A", "HTG3"];
    public override string DefaultTagsCsv => "shelly, iot, sensor, temperature, humidity, gen3";
    public override string Description => "Temperatur- och luftfuktighetssensor med klocka och e-ink-display (Gen3)";
    public override string ProductUrl => "https://www.shelly.com/collections/sensors-and-thermostats";
}

public sealed class ShellyWallDisplayProduct : ShellyProductDefinition
{
    public override string ProductId => "shelly-wall-display";
    public override string Model => "Wall Display";
    public override string SubCategory => "Sensors & Controllers";
    public override int Generation => 2;
    public override int RelayChannels => 1;
    public override IReadOnlyList<string> DeviceCodes => ["SAWD-0A1XX10EU1", "WallDisplay"];
    public override string DefaultTagsCsv => "shelly, iot, display, thermostat, relay, gen2";
    public override string Description => "4-tums smart kontrollpanel med inbyggt 5A relä och termostat";
    public override string ProductUrl => "https://www.shelly.com/collections/smart-controllers";
}
