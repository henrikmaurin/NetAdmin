using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Apple.Products;

public sealed class AppleIphoneProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-iphone";
    public override string Vendor => "Apple";
    public override string Model => "iPhone";
    public override HardwareCategory Category => HardwareCategory.Phone;
    public override string SubCategory => "Smartphones";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["iPhone"];
    public override string DefaultTagsCsv => "phone, apple, ios, wifi";
    public override string Description => "Apple iPhone smartphone (Wi-Fi client)";
}

public sealed class AppleIpadProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-ipad";
    public override string Vendor => "Apple";
    public override string Model => "iPad";
    public override HardwareCategory Category => HardwareCategory.Tablet;
    public override string SubCategory => "Tablets";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["iPad"];
    public override string DefaultTagsCsv => "tablet, apple, ipados, wifi";
    public override string Description => "Apple iPad tablet (Wi-Fi client)";
}

public sealed class AppleTv4KProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-tv-4k";
    public override string Vendor => "Apple";
    public override string Model => "Apple TV 4K (Wi-Fi + Ethernet)";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Apple-TV", "AppleTV"];
    public override string DefaultTagsCsv => "mediaplayer, apple, appletv, matter, thread, lan";
    public override string Description => "Apple TV 4K media player with Gigabit Ethernet, Matter Hub & Thread Border Router";
}

public sealed class AppleMacBookProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-macbook";
    public override string Vendor => "Apple";
    public override string Model => "MacBook Air / Pro";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Laptops";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["MacBook", "MacBook-Pro", "MacBook-Air"];
    public override string DefaultTagsCsv => "computer, laptop, apple, macos, wifi";
    public override string Description => "Apple MacBook laptop computer (Wi-Fi)";
}

public sealed class AppleMacDesktopProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-mac-desktop";
    public override string Vendor => "Apple";
    public override string Model => "Mac mini / iMac / Mac Studio";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Computers & Workstations";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Mac-mini", "iMac", "Mac-Studio"];
    public override string DefaultTagsCsv => "computer, desktop, apple, macos, lan";
    public override string Description => "Apple desktop computer with built-in RJ45 Gigabit/10Gb Ethernet";
}

public sealed class AppleHomePodMiniProduct : IoTHubProductDefinition
{
    public override string ProductId => "apple-homepod-mini";
    public override string Vendor => "Apple";
    public override string Model => "HomePod mini (HomeKit / Matter Hub)";
    public override int DefaultPortCount => 0;
    public override string SupportedProtocolsCsv => "Matter, Thread, Wi-Fi, Bluetooth LE";
    public override IReadOnlyList<string> DeviceCodes => ["HomePod", "HomePod-mini"];
    public override string DefaultTagsCsv => "iothub, apple, homekit, matter, thread, wifi";
    public override string Description => "Apple HomePod mini Smart Speaker, HomeKit Hub, Matter Controller & Thread Border Router";
}

public sealed class AppleHomePodMiniSmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-homepod-mini-speaker";
    public override string Vendor => "Apple";
    public override string Model => "HomePod mini";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["HomePod-mini", "HomePod"];
    public override string DefaultTagsCsv => "smartspeaker, apple, homepod, airplay2, siri, matter, thread, wifi";
    public override string Description => "Apple HomePod mini smart speaker with Siri, AirPlay 2, Matter & Thread";
}

public sealed class AppleHomePod2SmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-homepod-2";
    public override string Vendor => "Apple";
    public override string Model => "HomePod (2nd Gen)";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["HomePod-2"];
    public override string DefaultTagsCsv => "smartspeaker, apple, homepod, airplay2, dolby-atmos, siri, matter, thread, wifi";
    public override string Description => "Apple HomePod (2nd Generation) high-fidelity smart speaker with Spatial Audio, Siri & AirPlay 2";
}

public sealed class AppleWatchProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-watch";
    public override string Vendor => "Apple";
    public override string Model => "Apple Watch (Series / SE)";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Watch", "Apple-Watch", "Watch-SE", "Watch-Series"];
    public override string DefaultTagsCsv => "watch, apple, watchos, bluetooth, wifi";
    public override string Description => "Apple Watch series smartwatch (Bluetooth, Wi-Fi, optional LTE)";
}

public sealed class AppleWatchSeries9Product : HardwareProductDefinition
{
    public override string ProductId => "apple-watch-series-9";
    public override string Vendor => "Apple";
    public override string Model => "Apple Watch Series 9";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Watch-Series-9", "Watch9", "Watch-Series"];
    public override string DefaultTagsCsv => "watch, apple, watchos, series9, bluetooth, wifi";
    public override string Description => "Apple Watch Series 9 smartwatch (WatchOS, Bluetooth, Wi‑Fi, optional LTE)";
}

public sealed class AppleWatchSEProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-watch-se";
    public override string Vendor => "Apple";
    public override string Model => "Apple Watch SE";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Watch-SE", "Watch-SE-2", "WatchSE"];
    public override string DefaultTagsCsv => "watch, apple, se, watchos, bluetooth, wifi";
    public override string Description => "Apple Watch SE more affordable smartwatch variant (Bluetooth, Wi‑Fi)";
}

public sealed class AppleWatchUltraProduct : HardwareProductDefinition
{
    public override string ProductId => "apple-watch-ultra";
    public override string Vendor => "Apple";
    public override string Model => "Apple Watch Ultra";
    public override HardwareCategory Category => HardwareCategory.SmartWatch;
    public override string SubCategory => "Smart Watches";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Watch-Ultra", "WatchUltra"];
    public override string DefaultTagsCsv => "watch, apple, ultra, watchos, bluetooth, wifi";
    public override string Description => "Apple Watch Ultra rugged smartwatch with advanced sensors (Bluetooth, Wi‑Fi, optional LTE)";
}
