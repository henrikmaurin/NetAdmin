using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Samsung.Products;

public sealed class SamsungGalaxyWatchProduct : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-watch";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Watch (Active / Classic)";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Watch", "SM-R", "Galaxy-Watch-5"];
    public override string DefaultTagsCsv => "watch, samsung, wearable, bluetooth, wifi";
    public override string Description => "Samsung Galaxy Watch series smartwatch (Tizen / Wear OS depending on model; Bluetooth, Wi-Fi, optional LTE)";
}

public sealed class SamsungGalaxyWatch4Product : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-watch4";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Watch4";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Watch4", "SM-R885", "SM-R870"];
    public override string DefaultTagsCsv => "watch, samsung, wearable, bluetooth, wifi";
    public override string Description => "Samsung Galaxy Watch4 smartwatch (Bluetooth, Wi‑Fi, optional LTE)";
}

public sealed class SamsungGalaxyWatch5Product : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-watch5";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Watch5";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Watch5", "SM-R910"];
    public override string DefaultTagsCsv => "watch, samsung, wearable, bluetooth, wifi";
    public override string Description => "Samsung Galaxy Watch5 smartwatch (Bluetooth, Wi‑Fi, optional LTE)";
}

public sealed class SamsungGalaxyWatch6Product : HardwareProductDefinition
{
    public override string ProductId => "samsung-galaxy-watch6";
    public override string Vendor => "Samsung";
    public override string Model => "Galaxy Watch6";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Galaxy-Watch6", "SM-R92X"];
    public override string DefaultTagsCsv => "watch, samsung, wearable, bluetooth, wifi";
    public override string Description => "Samsung Galaxy Watch6 smartwatch (Bluetooth, Wi‑Fi, optional LTE)";
}
