using NetAdmin.Core.Models;

namespace NetAdmin.Providers.PfSense.Products;

public abstract class PfSenseRouterProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "pfSense";
    public override HardwareCategory Category => HardwareCategory.Router;
    public override string SubCategory => "Routers & Firewalls";
}

public sealed class PfSenseApplianceProduct : PfSenseRouterProductDefinition
{
    public override string ProductId => "pfsense-firewall-appliance";
    public override string Model => "pfSense CE / Plus Firewall Appliance";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["pfSense", "pfSense-CE", "pfSense-Plus"];
    public override string DefaultTagsCsv => "router, firewall, pfsense, vlan, 2.5gbe";
    public override string Description => "pfSense Community Edition / Plus Firewall & Router Appliance (4 physical NIC ports)";
}

public sealed class Netgate1100Product : PfSenseRouterProductDefinition
{
    public override string ProductId => "netgate-1100-pfsense";
    public override string Model => "Netgate 1100 (pfSense+)";
    public override string SubCategory => "Netgate Security Gateways";
    public override int DefaultPortCount => 3;
    public override IReadOnlyList<string> DeviceCodes => ["SG-1100", "Netgate 1100"];
    public override string DefaultTagsCsv => "router, firewall, pfsense, netgate, vlan";
    public override string Description => "Compact pfSense+ Security Gateway with 3x GbE ports (WAN, LAN, OPT)";
}

public sealed class Netgate2100Product : PfSenseRouterProductDefinition
{
    public override string ProductId => "netgate-2100-pfsense";
    public override string Model => "Netgate 2100 (pfSense+)";
    public override string SubCategory => "Netgate Security Gateways";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["SG-2100", "Netgate 2100"];
    public override string DefaultTagsCsv => "router, firewall, pfsense, netgate, vlan";
    public override string Description => "pfSense+ Security Gateway with 1x GbE WAN + 4-port GbE Marvell switch";
}

public sealed class Netgate4200Product : PfSenseRouterProductDefinition
{
    public override string ProductId => "netgate-4200-pfsense";
    public override string Model => "Netgate 4200 (pfSense+)";
    public override string SubCategory => "Netgate Security Gateways";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["Netgate 4200", "SG-4200"];
    public override string DefaultTagsCsv => "router, firewall, pfsense, netgate, vlan, 2.5gbe";
    public override string Description => "High-performance pfSense+ Security Gateway with 4x 2.5GbE ports";
}

public sealed class Netgate6100Product : PfSenseRouterProductDefinition
{
    public override string ProductId => "netgate-6100-pfsense";
    public override string Model => "Netgate 6100 (pfSense+)";
    public override string SubCategory => "Netgate Security Gateways";
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["Netgate 6100", "SG-6100"];
    public override string DefaultTagsCsv => "router, firewall, pfsense, netgate, vlan, 10gbe, sfp+";
    public override string Description => "Enterprise pfSense+ Security Gateway with 2x 10G SFP+, 2x 1G Combo, and 4x 2.5GbE ports";
}
