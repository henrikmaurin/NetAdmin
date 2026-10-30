using NetAdmin.Core.Models;

namespace NetAdmin.Providers.OpnSense.Products;

public abstract class RouterProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.Router;
    public override string SubCategory => "Routers & Firewalls";
}

public sealed class OpnSenseApplianceProduct : RouterProductDefinition
{
    public override string ProductId => "opnsense-firewall-appliance";
    public override string Vendor => "OPNsense";
    public override string Model => "N100 / N150 4x2.5GbE Firewall";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["OPNsense", "N100-4P"];
    public override string DefaultTagsCsv => "router, firewall, opnsense, vlan, 2.5gbe";
    public override string Description => "OPNsense Firewall & Router Appliance (4 physical 2.5GbE ports)";
}

public sealed class ZyxelNr5103E5GRouterProduct : RouterProductDefinition
{
    public override string ProductId => "zyxel-nr5103e-5g";
    public override string Vendor => "Zyxel";
    public override string Model => "NR5103E 5G NR Indoor Router";
    public override string SubCategory => "5G Mobile Routers";
    public override int DefaultPortCount => 2;
    public override IReadOnlyList<string> DeviceCodes => ["NR5103E", "NR5103"];
    public override string DefaultTagsCsv => "router, 5g, wan, failover, zyxel, wifi6";
    public override string Description => "5G NR Indoor Wi-Fi 6 Router with 2x 2.5GbE ports & IP Passthrough / Bridge Mode";
}

public sealed class ZyxelNr71015GOutdoorRouterProduct : RouterProductDefinition
{
    public override string ProductId => "zyxel-nr7101-5g";
    public override string Vendor => "Zyxel";
    public override string Model => "NR7101 5G NR Outdoor Router";
    public override string SubCategory => "5G Mobile Routers";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["NR7101", "Zyxel5G"];
    public override string DefaultTagsCsv => "router, 5g, outdoor, poe, wan, failover, zyxel";
    public override string Description => "5G NR Outdoor PoE Router / Modem (IP68, Bridge / IP Passthrough)";
}

public sealed class ZyxelFwa5055GRouterProduct : RouterProductDefinition
{
    public override string ProductId => "zyxel-fwa505-5g";
    public override string Vendor => "Zyxel";
    public override string Model => "Nebula FWA505 5G Router";
    public override string SubCategory => "5G Mobile Routers";
    public override int DefaultPortCount => 2;
    public override IReadOnlyList<string> DeviceCodes => ["FWA505", "FWA510"];
    public override string DefaultTagsCsv => "router, 5g, wan, failover, zyxel";
    public override string Description => "Indoor 5G NR Wi-Fi 6 Router with 2x GbE LAN/WAN ports";
}

public sealed class GenericRouterProduct : RouterProductDefinition
{
    public override string ProductId => "generic-router-firewall";
    public override string Vendor => "Generic";
    public override string Model => "Router / Firewall";
    public override string SubCategory => "Generic Router";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["GenericRouter"];
    public override string DefaultTagsCsv => "router, firewall";
    public override string Description => "Generic router or firewall with 4 network ports";
}
