using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Ikea.Products;

public sealed class IkeaTradfriGatewayProduct : IoTHubProductDefinition
{
    public override string ProductId => "ikea-tradfri-gateway-e1526";
    public override string Vendor => "IKEA";
    public override string Model => "TRÅDFRI Gateway (E1526)";
    public override int DefaultPortCount => 1;
    public override string SupportedProtocolsCsv => "Zigbee, Ethernet (LAN), CoAP";
    public override IReadOnlyList<string> DeviceCodes => ["E1526", "TRADFRI-Gateway", "Tradfri", "GW-"];
    public override string DefaultTagsCsv => "iothub, ikea, tradfri, zigbee, lan";
    public override string Description => "IKEA TRÅDFRI Zigbee Gateway with physical RJ45 Ethernet port (CoAP / Home Assistant)";
}

public sealed class IkeaDirigeraHubProduct : IoTHubProductDefinition
{
    public override string ProductId => "ikea-dirigera-hub-e2003";
    public override string Vendor => "IKEA";
    public override string Model => "DIRIGERA Hub (E2003)";
    public override int DefaultPortCount => 1;
    public override string SupportedProtocolsCsv => "Zigbee, Matter, Thread, Ethernet (LAN), Wi-Fi, Bluetooth LE";
    public override IReadOnlyList<string> DeviceCodes => ["DIRIGERA", "E2003"];
    public override string DefaultTagsCsv => "iothub, ikea, dirigera, zigbee, matter, thread, lan";
    public override string Description => "IKEA DIRIGERA Smart Home Hub with RJ45 Ethernet, Zigbee, Matter Bridge & Thread Border Router";
}

public sealed class IkeaSymfoniskBookshelfSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "ikea-symfonisk-speaker";
    public override string Vendor => "IKEA";
    public override string Model => "SYMFONISK Wi-Fi Speaker (Bookshelf / Lamp / Frame)";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Connected Speakers";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["SYMFONISK"];
    public override string DefaultTagsCsv => "soundbar, speaker, ikea, sonos, airplay2, lan, wifi";
    public override string Description => "IKEA / Sonos SYMFONISK Wi-Fi & Ethernet multi-room connected speaker with AirPlay 2";
}
