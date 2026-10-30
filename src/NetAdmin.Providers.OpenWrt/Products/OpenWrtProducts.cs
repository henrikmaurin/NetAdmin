using NetAdmin.Core.Models;

namespace NetAdmin.Providers.OpenWrt.Products;

public abstract class OpenWrtRouterProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "OpenWrt";
    public override HardwareCategory Category => HardwareCategory.Router;
    public override string SubCategory => "OpenWrt Routers";
}

public sealed class OpenWrtOneProduct : OpenWrtRouterProductDefinition
{
    public override string ProductId => "openwrt-one";
    public override string Model => "OpenWrt One";
    public override int DefaultPortCount => 2;
    public override IReadOnlyList<string> DeviceCodes => ["OpenWrt One", "AP-24.XY"];
    public override string DefaultTagsCsv => "router, openwrt, wifi6, 2.5gbe, poe";
    public override string Description => "Official OpenWrt Wi-Fi 6 Router with 1x 2.5GbE PoE PD WAN + 1x GbE LAN";
}

public sealed class OpenWrtFlint2Product : OpenWrtRouterProductDefinition
{
    public override string ProductId => "openwrt-gl-mt6000-flint2";
    public override string Model => "GL.iNet Flint 2 (GL-MT6000)";
    public override int DefaultPortCount => 6;
    public override IReadOnlyList<string> DeviceCodes => ["GL-MT6000", "Flint 2"];
    public override string DefaultTagsCsv => "router, openwrt, wifi6, dual-2.5gbe, vlan";
    public override string Description => "AX6000 OpenWrt Wi-Fi 6 Router with 2x 2.5GbE + 4x GbE ports";
}

public sealed class OpenWrtGenericRouterProduct : OpenWrtRouterProductDefinition
{
    public override string ProductId => "openwrt-generic-router";
    public override string Model => "OpenWrt Router / Appliance";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["OpenWrt"];
    public override string DefaultTagsCsv => "router, openwrt, vlan, firewall";
    public override string Description => "Router or x86 appliance running OpenWrt firmware";
}
