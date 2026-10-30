using NetAdmin.Core.Models;

namespace NetAdmin.Providers.UniFi.Products;

public abstract class UniFiAccessPointProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "Ubiquiti UniFi";
    public override HardwareCategory Category => HardwareCategory.AccessPoint;
    public override string SubCategory => "Wi-Fi Access Points";
    public override int DefaultPortCount => 1;
}

public sealed class UniFiU6ProProduct : UniFiAccessPointProductDefinition
{
    public override string ProductId => "unifi-u6-pro";
    public override string Model => "U6 Pro";
    public override IReadOnlyList<string> DeviceCodes => ["U6-Pro", "U6Pro"];
    public override string DefaultTagsCsv => "ap, wifi6, unifi, poe";
    public override string Description => "Dual-band Wi-Fi 6 Access Point (5.3 Gbps, GbE PoE)";
}

public sealed class UniFiU6LiteProduct : UniFiAccessPointProductDefinition
{
    public override string ProductId => "unifi-u6-lite";
    public override string Model => "U6 Lite / U6+";
    public override IReadOnlyList<string> DeviceCodes => ["U6-Lite", "U6-Plus", "U6+"];
    public override string DefaultTagsCsv => "ap, wifi6, unifi, poe";
    public override string Description => "Compact Dual-band Wi-Fi 6 Access Point (GbE PoE)";
}

public sealed class UniFiU7ProProduct : UniFiAccessPointProductDefinition
{
    public override string ProductId => "unifi-u7-pro";
    public override string Model => "U7 Pro";
    public override IReadOnlyList<string> DeviceCodes => ["U7-Pro", "U7Pro"];
    public override string DefaultTagsCsv => "ap, wifi7, 6ghz, unifi, poe+";
    public override string Description => "Tri-band Wi-Fi 7 Access Point with 6 GHz and 2.5 GbE PoE+";
}

public sealed class UniFiU6MeshProduct : UniFiAccessPointProductDefinition
{
    public override string ProductId => "unifi-u6-mesh";
    public override string Model => "U6 Mesh";
    public override IReadOnlyList<string> DeviceCodes => ["U6-Mesh"];
    public override string DefaultTagsCsv => "ap, wifi6, mesh, outdoor, unifi, poe";
    public override string Description => "IPX5-rated Indoor/Outdoor Wi-Fi 6 Mesh Access Point";
}

public sealed class GenericAccessPointProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-wifi-ap";
    public override string Vendor => "Generic";
    public override string Model => "Wi-Fi Access Point";
    public override HardwareCategory Category => HardwareCategory.AccessPoint;
    public override string SubCategory => "Generic Access Point";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericAP"];
    public override string DefaultTagsCsv => "ap, wifi";
    public override string Description => "Generic wireless access point with 1 RJ45 uplink port";
}
