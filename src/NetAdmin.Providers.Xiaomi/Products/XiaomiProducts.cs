using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Xiaomi.Products;

public sealed class XiaomiWatch2ProProduct : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-watch-2-pro";
    public override string Vendor => "XiaoMi";
    public override string Model => "Watch 2 Pro";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["XiaoMi-Watch-2-Pro", "Watch2Pro", "XMWatch2Pro"];
    public override string DefaultTagsCsv => "watch, xiaomi, wearable, bluetooth, wifi";
    public override string Description => "XiaoMi Watch 2 Pro smartwatch (Bluetooth, Wi-Fi, GPS)";
}

public sealed class XiaomiWatch2Product : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-watch-2";
    public override string Vendor => "XiaoMi";
    public override string Model => "Watch 2";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["XiaoMi-Watch-2", "Watch2", "XMWatch2"];
    public override string DefaultTagsCsv => "watch, xiaomi, wearable, bluetooth, wifi";
    public override string Description => "XiaoMi Watch 2 smartwatch (Bluetooth, Wi‑Fi, GPS)";
}

public sealed class XiaomiWatchS1Product : HardwareProductDefinition
{
    public override string ProductId => "xiaomi-watch-s1";
    public override string Vendor => "XiaoMi";
    public override string Model => "Watch S1 / S1 Pro";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Watch-S1", "Watch-S1-Pro", "XMS1"];
    public override string DefaultTagsCsv => "watch, xiaomi, s1, wearable, bluetooth, wifi";
    public override string Description => "XiaoMi Watch S1 series smartwatch (Bluetooth, Wi‑Fi, optional GPS)";
}
