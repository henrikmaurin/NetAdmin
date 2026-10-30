using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Yamaha.Products;

public sealed class YamahaYas109SoundbarProduct : HardwareProductDefinition
{
    public override string ProductId => "yamaha-yas-109";
    public override string Vendor => "Yamaha";
    public override string Model => "YAS-109 Soundbar";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Soundbars";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["YAS-109", "YAS109", "Yamaha-YAS-109"];
    public override string DefaultTagsCsv => "soundbar, yamaha, yas-109, spotify-connect, alexa, lan, wifi";
    public override string Description => "Yamaha YAS-109 Soundbar with built-in subwoofers, Wi-Fi, RJ45 Ethernet, Spotify Connect & Alexa";
}

public sealed class YamahaYas209SoundbarProduct : HardwareProductDefinition
{
    public override string ProductId => "yamaha-yas-209";
    public override string Vendor => "Yamaha";
    public override string Model => "YAS-209 / SR-B20A / SR-B40A Soundbar";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Soundbars";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["YAS-209", "SR-B20A", "SR-B40A", "SR-X40A", "YSP-"];
    public override string DefaultTagsCsv => "soundbar, yamaha, spotify-connect, lan, wifi";
    public override string Description => "Yamaha Soundbar with wireless subwoofer, Wi-Fi, Ethernet & streaming audio";
}

public sealed class YamahaMusicCastAvReceiverProduct : HardwareProductDefinition
{
    public override string ProductId => "yamaha-rx-v-aventage-avr";
    public override string Vendor => "Yamaha";
    public override string Model => "RX-V / AVENTAGE MusicCast AV Receiver";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "AV Receivers & Connected Audio";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["RX-V", "RX-A", "TSR-", "MusicCast"];
    public override string DefaultTagsCsv => "soundbar, avr, yamaha, musiccast, airplay2, spotify-connect, lan, wifi";
    public override string Description => "Yamaha MusicCast Network AV Receiver with Ethernet, Wi-Fi, AirPlay 2 & Spotify Connect";
}

public sealed class YamahaMusicCastSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "yamaha-musiccast-20-50";
    public override string Vendor => "Yamaha";
    public override string Model => "MusicCast 20 / MusicCast 50 (WX-021 / WX-051)";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Connected Speakers";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["WX-021", "WX-051", "MusicCast-20", "MusicCast-50"];
    public override string DefaultTagsCsv => "soundbar, speaker, yamaha, musiccast, airplay2, lan, wifi";
    public override string Description => "Yamaha MusicCast wireless multi-room speaker with Wi-Fi, Ethernet & AirPlay 2";
}
