using NetAdmin.Core.Models;

namespace NetAdmin.Providers.HomeAssistant.Products;

public abstract class HomeAutomationProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.HomeAutomation;
    public override string SubCategory => "Home Automation Controllers";
    public override int DefaultPortCount => 1;
}

public sealed class HomeAssistantGreenProduct : HomeAutomationProductDefinition
{
    public override string ProductId => "home-assistant-green";
    public override string Vendor => "Home Assistant";
    public override string Model => "Home Assistant Green";
    public override IReadOnlyList<string> DeviceCodes => ["ha-green", "homeassistant-green"];
    public override string DefaultTagsCsv => "homeautomation, homeassistant, green, lan";
    public override string Description => "Official quad-core ARM smart home appliance running Home Assistant OS with Gigabit Ethernet";
    public override string ProductUrl => "https://www.home-assistant.io/green/";
}

public sealed class HomeAssistantYellowProduct : HomeAutomationProductDefinition
{
    public override string ProductId => "home-assistant-yellow";
    public override string Vendor => "Home Assistant";
    public override string Model => "Home Assistant Yellow (CM4 / PoE)";
    public override IReadOnlyList<string> DeviceCodes => ["ha-yellow", "homeassistant-yellow"];
    public override string DefaultTagsCsv => "homeautomation, homeassistant, yellow, zigbee, thread, matter, poe, lan";
    public override string Description => "Modular Home Assistant smart home controller with integrated Zigbee / Thread radio, NVMe slot, and optional PoE";
    public override string ProductUrl => "https://www.home-assistant.io/yellow/";
}

public sealed class HomeAssistantOsVirtualApplianceProduct : HomeAutomationProductDefinition
{
    public override string ProductId => "home-assistant-os-vm";
    public override string Vendor => "Home Assistant";
    public override string Model => "Home Assistant OS (VM / Mini-PC / Container)";
    public override IReadOnlyList<string> DeviceCodes => ["haos", "homeassistant"];
    public override string DefaultTagsCsv => "homeautomation, homeassistant, haos, vm, docker";
    public override string Description => "Self-hosted Home Assistant OS virtual machine or container deployment";
    public override string ProductUrl => "https://www.home-assistant.io/";
}
