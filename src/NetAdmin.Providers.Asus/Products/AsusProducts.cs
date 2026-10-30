using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Asus.Products;

public abstract class AsusRouterProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "Asus";
    public override HardwareCategory Category => HardwareCategory.Router;
    public override string SubCategory => "Wi-Fi 6 / 7 Routers";
}

public sealed class AsusRtAx86UProRouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-ax86u-pro";
    public override string Model => "RT-AX86U Pro";
    public override string SubCategory => "Wi-Fi 6 Routers";
    public override int DefaultPortCount => 6;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX86U Pro", "RT-AX86U"];
    public override string DefaultTagsCsv => "router, asus, wifi6, 2.5gbe, aimesh";
    public override string Description => "Dual-Band Wi-Fi 6 AX5700 Gaming Router with 2.5G WAN/LAN + 4x GbE LAN";
}

public sealed class AsusRtAx88UProRouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-ax88u-pro";
    public override string Model => "RT-AX88U Pro";
    public override string SubCategory => "Wi-Fi 6 Routers";
    public override int DefaultPortCount => 6;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX88U Pro", "RT-AX88U"];
    public override string DefaultTagsCsv => "router, asus, wifi6, dual-2.5gbe, aimesh, vlan";
    public override string Description => "AX6000 Dual-Band Wi-Fi 6 Router with Dual 2.5GbE ports, VLAN support & 4x GbE LAN";
}

public sealed class AsusRtAx58URouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-ax58u";
    public override string Model => "RT-AX58U / RT-AX3000";
    public override string SubCategory => "Wi-Fi 6 Routers";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX58U", "RT-AX3000"];
    public override string DefaultTagsCsv => "router, asus, wifi6, aimesh";
    public override string Description => "AX3000 Dual-Band Wi-Fi 6 Router (1x WAN + 4x GbE LAN, AiMesh)";
}

public sealed class AsusRtAx55RouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-ax55";
    public override string Model => "RT-AX55 / RT-AX1800U";
    public override string SubCategory => "Wi-Fi 6 Routers";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX55", "RT-AX1800U", "RT-AX56U"];
    public override string DefaultTagsCsv => "router, asus, wifi6, aimesh";
    public override string Description => "AX1800 Dual-Band Wi-Fi 6 Router (1x GbE WAN + 4x GbE LAN, AiMesh)";
}

public sealed class AsusRtAx82URouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-ax82u";
    public override string Model => "RT-AX82U / TUF Gaming AX5400";
    public override string SubCategory => "Wi-Fi 6 Gaming Routers";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX82U", "TUF-AX5400", "TUF-AX3000"];
    public override string DefaultTagsCsv => "router, asus, wifi6, gaming, aimesh";
    public override string Description => "AX5400 Dual-Band Wi-Fi 6 Gaming Router (1x GbE WAN + 4x GbE LAN, AiMesh)";
}

public sealed class AsusTufAx6000RouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-tuf-ax6000";
    public override string Model => "TUF Gaming AX6000";
    public override string SubCategory => "Wi-Fi 6 Gaming Routers";
    public override int DefaultPortCount => 6;
    public override IReadOnlyList<string> DeviceCodes => ["TUF-AX6000"];
    public override string DefaultTagsCsv => "router, asus, tuf, wifi6, dual-2.5gbe, aimesh";
    public override string Description => "AX6000 Dual-Band Wi-Fi 6 Gaming Router with Dual 2.5GbE ports & 4x GbE LAN";
}

public sealed class AsusGtAx6000RouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-gt-ax6000";
    public override string Model => "ROG Rapture GT-AX6000";
    public override string SubCategory => "Wi-Fi 6 Gaming Routers";
    public override int DefaultPortCount => 6;
    public override IReadOnlyList<string> DeviceCodes => ["GT-AX6000"];
    public override string DefaultTagsCsv => "router, asus, rog, wifi6, dual-2.5gbe, aimesh";
    public override string Description => "ROG Rapture AX6000 Wi-Fi 6 Gaming Router with Dual 2.5GbE ports & 4x GbE LAN";
}

public sealed class AsusZenWifiXt8RouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-zenwifi-xt8";
    public override string Model => "ZenWiFi AX (XT8)";
    public override string SubCategory => "Mesh Wi-Fi Routers";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["XT8", "ZenWiFi XT8"];
    public override string DefaultTagsCsv => "router, asus, zenwifi, wifi6, tri-band, aimesh";
    public override string Description => "AX6600 Tri-Band Mesh Wi-Fi 6 System / Router (1x 2.5G WAN + 3x GbE LAN)";
}

public sealed class AsusRtBe88URouterProduct : AsusRouterProductDefinition
{
    public override string ProductId => "asus-rt-be88u";
    public override string Model => "RT-BE88U (Wi-Fi 7)";
    public override string SubCategory => "Wi-Fi 7 Routers";
    public override int DefaultPortCount => 10;
    public override IReadOnlyList<string> DeviceCodes => ["RT-BE88U", "RT-BE96U"];
    public override string DefaultTagsCsv => "router, asus, wifi7, 10gbe, 2.5gbe, vlan, aimesh";
    public override string Description => "BE7200 Dual-Band Wi-Fi 7 Router with 10G WAN/LAN, SFP+, 4x 2.5GbE & 4x GbE ports";
}

public sealed class AsusWifiAccessPointProduct : HardwareProductDefinition
{
    public override string ProductId => "asus-wifi-ap";
    public override string Vendor => "Asus";
    public override string Model => "RT-AX / ZenWiFi AiMesh Access Point";
    public override HardwareCategory Category => HardwareCategory.AccessPoint;
    public override string SubCategory => "Wi-Fi Access Points";
    public override int DefaultPortCount => 4;
    public override IReadOnlyList<string> DeviceCodes => ["RT-AX58U", "RT-AX86U", "AsusAP", "AiMesh"];
    public override string DefaultTagsCsv => "ap, wifi6, asus, aimesh";
    public override string Description => "Asus Wi-Fi 6 Router in Access Point / AiMesh Node mode (with 4 built-in switch ports)";
}
