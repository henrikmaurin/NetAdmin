using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Xiaomi.Products;

public sealed class XiaomiSmartHomeHub2Product : IoTHubProductDefinition
{
    public override string ProductId => "xiaomi-smart-home-hub-2";
    public override string Vendor => "Xiaomi";
    public override string Model => "Smart Home Hub 2 (ZNDMWG04LM)";
    public override int DefaultPortCount => 1;
    public override string SupportedProtocolsCsv => "Bluetooth LE, Bluetooth Mesh, Zigbee, Ethernet (LAN), Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["ZNDMWG04LM", "lumi.gateway.mgl001", "XiaomiHub2"];
    public override string DefaultTagsCsv => "iothub, xiaomi, bluetooth-mesh, ble, zigbee, lan";
    public override string Description => "Xiaomi Multi-Mode Smart Home Hub 2 with Bluetooth Mesh Gateway, BLE, Zigbee 3.0, RJ45 Ethernet & Dual-Band Wi-Fi";
}

public sealed class XiaomiMiSmartHomeHubProduct : IoTHubProductDefinition
{
    public override string ProductId => "xiaomi-mi-smart-home-hub";
    public override string Vendor => "Xiaomi";
    public override string Model => "Mi Smart Home Hub (ZNDMWG03LM)";
    public override int DefaultPortCount => 0;
    public override string SupportedProtocolsCsv => "Bluetooth LE, Bluetooth Mesh, Zigbee, Wi-Fi";
    public override IReadOnlyList<string> DeviceCodes => ["ZNDMWG03LM", "lumi.gateway.mgl03", "XiaomiBluetoothHub"];
    public override string DefaultTagsCsv => "iothub, xiaomi, bluetooth-mesh, ble, zigbee, wifi";
    public override string Description => "Xiaomi Bluetooth / Bluetooth Mesh & Zigbee 3.0 Smart Home Gateway (Wi-Fi)";
}

public sealed class XiaomiSmartphoneProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-smartphone";
    public override string Vendor => "Xiaomi";
    public override string Model => "Xiaomi / Redmi / POCO Smartphone";
    public override HardwareCategory Category => HardwareCategory.Phone;
    public override string SubCategory => "Smartphones";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Xiaomi", "Redmi", "POCO"];
    public override string DefaultTagsCsv => "phone, xiaomi, android, wifi";
    public override string Description => "Xiaomi / Redmi / POCO Android smartphone (Wi-Fi client)";
}

public sealed class XiaomiPadProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-pad";
    public override string Vendor => "Xiaomi";
    public override string Model => "Xiaomi Pad / Redmi Pad";
    public override HardwareCategory Category => HardwareCategory.Tablet;
    public override string SubCategory => "Tablets";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Xiaomi-Pad", "Redmi-Pad", "Mi-Pad"];
    public override string DefaultTagsCsv => "tablet, xiaomi, android, wifi";
    public override string Description => "Xiaomi Pad / Redmi Pad Android tablet (Wi-Fi client)";
}

public sealed class XiaomiSmartTvProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-smart-tv";
    public override string Vendor => "Xiaomi";
    public override string Model => "Xiaomi Smart TV (Google TV)";
    public override HardwareCategory Category => HardwareCategory.Tv;
    public override string SubCategory => "Smart TVs";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["MiTV", "Xiaomi-TV"];
    public override string DefaultTagsCsv => "tv, xiaomi, google-tv, media, wifi";
    public override string Description => "Xiaomi Smart TV with Google TV, RJ45 LAN & Wi-Fi";
}

public sealed class XiaomiMiBoxMediaPlayerProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-mi-box-s";
    public override string Vendor => "Xiaomi";
    public override string Model => "TV Box S (2nd Gen) / Mi TV Stick 4K";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["MiBOX", "MiBoxS", "Xiaomi-TV-Box", "MiTV-Stick"];
    public override string DefaultTagsCsv => "mediaplayer, xiaomi, google-tv, media, wifi";
    public override string Description => "Xiaomi TV Box S (2nd Gen) / Mi Box S 4K Google TV streaming media player";
}

public sealed class XiaomiRobotVacuumProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-robot-vacuum";
    public override string Vendor => "Xiaomi";
    public override string Model => "Robot Vacuum (Mi / Roborock / Dreame)";
    public override HardwareCategory Category => HardwareCategory.IoT;
    public override string SubCategory => "Smart Home Appliances";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["roborock", "rockrobo", "dreame", "viomi", "xiaomi-vacuum"];
    public override string DefaultTagsCsv => "iot, xiaomi, vacuum, wifi";
    public override string Description => "Xiaomi / Roborock / Dreame Wi-Fi Robot Vacuum Cleaner";
}

public sealed class XiaomiSmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-smart-speaker-ir";
    public override string Vendor => "Xiaomi";
    public override string Model => "Mi Smart Speaker / Smart Speaker IR Control";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Mi-Smart-Speaker", "Xiaomi-Smart-Speaker"];
    public override string DefaultTagsCsv => "smartspeaker, xiaomi, google-assistant, cast, wifi";
    public override string Description => "Xiaomi Smart Speaker with Google Assistant & Chromecast built-in";
}
