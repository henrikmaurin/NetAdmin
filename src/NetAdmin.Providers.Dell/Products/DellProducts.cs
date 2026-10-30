using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Dell.Products;

public sealed class DellXpsLaptopProduct : HardwareProductDefinition
{
    public override string ProductId => "dell-xps-laptop";
    public override string Vendor => "Dell";
    public override string Model => "XPS Laptop (XPS 13 / 14 / 15 / 16 / 17)";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Laptops";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["DELL-XPS", "XPS-13", "XPS-14", "XPS-15", "XPS-16", "XPS-17", "XPS"];
    public override string DefaultTagsCsv => "computer, laptop, dell, xps, windows, wifi";
    public override string Description => "Dell XPS notebook computer (Wi-Fi / Thunderbolt dock)";
}

public sealed class DellXpsDesktopProduct : HardwareProductDefinition
{
    public override string ProductId => "dell-xps-desktop";
    public override string Vendor => "Dell";
    public override string Model => "XPS Desktop (8950 / 8960)";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Desktops & Workstations";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["XPS-Desktop", "XPS-8950", "XPS-8960"];
    public override string DefaultTagsCsv => "computer, desktop, dell, xps, windows, lan, wifi";
    public override string Description => "Dell XPS Desktop workstation with Gigabit/2.5Gb Ethernet & Wi-Fi";
}

public sealed class DellOptiPlexMicroProduct : HardwareProductDefinition
{
    public override string ProductId => "dell-optiplex-micro";
    public override string Vendor => "Dell";
    public override string Model => "OptiPlex Micro / Precision Workstation";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Desktops & Workstations";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["OptiPlex", "Precision"];
    public override string DefaultTagsCsv => "computer, desktop, dell, optiplex, lan";
    public override string Description => "Dell OptiPlex Micro / Precision desktop or compact home server";
}

public sealed class DellPowerEdgeServerProduct : HardwareProductDefinition
{
    public override string ProductId => "dell-poweredge-server";
    public override string Vendor => "Dell";
    public override string Model => "PowerEdge Server";
    public override HardwareCategory Category => HardwareCategory.Server;
    public override string SubCategory => "Servers & Virtualization";
    public override int DefaultPortCount => 2;
    public override IReadOnlyList<string> DeviceCodes => ["PowerEdge", "iDRAC"];
    public override string DefaultTagsCsv => "server, dell, poweredge, idrac, proxmox";
    public override string Description => "Dell PowerEdge rack/tower server with dual GbE/10GbE NICs & iDRAC";
}
