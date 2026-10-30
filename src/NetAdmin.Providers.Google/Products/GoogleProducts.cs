using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Google.Products;

public sealed class GooglePixelPhoneProduct : HardwareProductDefinition
{
    public override string ProductId => "google-pixel-phone";
    public override string Vendor => "Google";
    public override string Model => "Pixel Smartphone";
    public override HardwareCategory Category => HardwareCategory.Phone;
    public override string SubCategory => "Smartphones";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Pixel"];
    public override string DefaultTagsCsv => "phone, google, pixel, android, wifi";
    public override string Description => "Google Pixel Android smartphone (Wi-Fi client)";
}

public sealed class GooglePixelTabletProduct : HardwareProductDefinition
{
    public override string ProductId => "google-pixel-tablet";
    public override string Vendor => "Google";
    public override string Model => "Pixel Tablet";
    public override HardwareCategory Category => HardwareCategory.Tablet;
    public override string SubCategory => "Tablets";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Pixel-Tablet"];
    public override string DefaultTagsCsv => "tablet, google, pixel, android, wifi";
    public override string Description => "Google Pixel Tablet with Charging Speaker Dock (Wi-Fi)";
}

public sealed class GoogleChromecastTvProduct : HardwareProductDefinition
{
    public override string ProductId => "google-chromecast-tv";
    public override string Vendor => "Google";
    public override string Model => "Chromecast with Google TV";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Chromecast", "Google-TV"];
    public override string DefaultTagsCsv => "mediaplayer, google, chromecast, media, wifi";
    public override string Description => "Google Chromecast with Google TV 4K/HD streaming media player (Wi-Fi)";
}

public sealed class GoogleTvStreamerProduct : HardwareProductDefinition
{
    public override string ProductId => "google-tv-streamer-4k";
    public override string Vendor => "Google";
    public override string Model => "Google TV Streamer (4K)";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Google-TV-Streamer"];
    public override string DefaultTagsCsv => "mediaplayer, google, matter, thread, lan";
    public override string Description => "Google TV Streamer 4K with RJ45 Gigabit Ethernet, Matter Hub & Thread Border Router";
}

public sealed class GoogleNestHubProduct : IoTHubProductDefinition
{
    public override string ProductId => "google-nest-hub-2";
    public override string Vendor => "Google";
    public override string Model => "Nest Hub (2nd Gen) / Nest Hub Max";
    public override int DefaultPortCount => 0;
    public override string SupportedProtocolsCsv => "Matter, Thread, Wi-Fi, Bluetooth LE";
    public override IReadOnlyList<string> DeviceCodes => ["Nest-Hub", "Google-Home"];
    public override string DefaultTagsCsv => "iothub, google, nest, matter, thread, wifi";
    public override string Description => "Google Nest Hub smart display with built-in Matter Controller & Thread Border Router";
}

public sealed class GoogleNestAudioSmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "google-nest-audio";
    public override string Vendor => "Google";
    public override string Model => "Nest Audio";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Nest-Audio", "Google-Nest-Audio"];
    public override string DefaultTagsCsv => "smartspeaker, google, nest, cast, assistant, wifi";
    public override string Description => "Google Nest Audio wireless smart speaker with Google Assistant & Chromecast built-in";
}

public sealed class GoogleNestMiniSmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "google-nest-mini";
    public override string Vendor => "Google";
    public override string Model => "Nest Mini (2nd Gen) / Home Mini";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Nest-Mini", "Google-Nest-Mini", "Google-Home-Mini"];
    public override string DefaultTagsCsv => "smartspeaker, google, nest, cast, assistant, wifi";
    public override string Description => "Google Nest Mini compact Wi-Fi smart speaker with Google Assistant";
}
