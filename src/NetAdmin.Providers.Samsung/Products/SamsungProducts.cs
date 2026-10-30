using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Samsung.Products;

public sealed class SamsungGalaxyPhoneProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-phone";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Smartphone";
    public override HardwareCategory Category => HardwareCategory.Phone;
    public override string SubCategory => "Smartphones";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy", "SM-S", "SM-A", "SM-G", "SM-F"];
    public override string DefaultTagsCsv => "phone, samsung, android, wifi";
    public override string Description => "Samsung Galaxy Android smartphone (Wi-Fi client)";
}

public sealed class SamsungGalaxyTabProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-tab";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Tab";
    public override HardwareCategory Category => HardwareCategory.Tablet;
    public override string SubCategory => "Tablets";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Tab", "SM-X", "SM-T"];
    public override string DefaultTagsCsv => "tablet, samsung, android, wifi";
    public override string Description => "Samsung Galaxy Tab Android tablet (Wi-Fi client)";
}

public sealed class SamsungSmartTvProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-smart-tv";
    public override string Vendor => "Samsung";
    public override string Model => "Smart TV (QLED / OLED / The Frame)";
    public override HardwareCategory Category => HardwareCategory.Tv;
    public override string SubCategory => "Smart TVs & Media Players";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Samsung-TV", "Tizen", "QN", "QE", "UE"];
    public override string DefaultTagsCsv => "tv, samsung, tizen, smartthings, lan, wifi";
    public override string Description => "Samsung Tizen Smart TV (QLED / Neo QLED / OLED / The Frame) with RJ45 LAN & Wi-Fi";
}

public sealed class SamsungGalaxyBookProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-book";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Book";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Laptops";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Book"];
    public override string DefaultTagsCsv => "computer, laptop, samsung, windows, wifi";
    public override string Description => "Samsung Galaxy Book Windows laptop (Wi-Fi)";
}

public sealed class SamsungSmartThingsHubProduct : IoTHubProductDefinition
{
    public override string ProductId => "samsung-smartthings-hub-v3";
    public override string Vendor => "Samsung";
    public override string Model => "SmartThings Hub v3 / Aeotec Hub";
    public override int DefaultPortCount => 1;
    public override string SupportedProtocolsCsv => "Zigbee, Z-Wave, Matter, Thread, Ethernet (LAN), Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["SmartThings", "IM6001-V3P"];
    public override string DefaultTagsCsv => "iothub, samsung, smartthings, zigbee, zwave, matter, thread, lan";
    public override string Description => "Samsung SmartThings / Aeotec Smart Home Hub with Zigbee, Z-Wave, Matter, Thread & RJ45 Ethernet";
}

public sealed class SamsungQSeriesSoundbarProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-q-series-soundbar";
    public override string Vendor => "Samsung";
    public override string Model => "HW-Q990D / HW-Q990C / Q-Series Soundbar";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Soundbars";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["HW-Q990", "HW-Q930", "HW-Q800", "HW-S800", "Samsung-Soundbar"];
    public override string DefaultTagsCsv => "soundbar, samsung, dolby-atmos, q-symphony, smartthings, airplay2, chromecast, wifi";
    public override string Description => "Samsung Q-Series Dolby Atmos Soundbar with Wi-Fi, SmartThings, AirPlay 2, Chromecast & Spotify Connect";
}
