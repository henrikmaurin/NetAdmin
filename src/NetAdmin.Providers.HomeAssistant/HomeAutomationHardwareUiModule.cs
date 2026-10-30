using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.HomeAssistant.Components;

namespace NetAdmin.Providers.HomeAssistant;

public class HomeAutomationHardwareUiModule : HardwareUiModuleBase
{
    public override string ModuleId => "homeautomation-ha";
    public override string DisplayName => "🏡 Home Automation";
    public override string Icon => "🏡";
    public override HardwareCategory Category => HardwareCategory.HomeAutomation;
    public override int Priority => 85;

    public override Type? EditorComponentType => typeof(HomeAutomationEditor);
    public override Type? CardSummaryComponentType => typeof(HomeAutomationCardSummary);

    protected override IReadOnlyList<string> MatchKeywords => ["homeassistant", "home assistant", "nabu casa", "haos"];
    protected override string DefaultDeviceName => "Home Assistant";
    protected override string DefaultRoom => "Server Closet";
    protected override string DefaultVendor => "Home Assistant";
    protected override string DefaultModel => "Home Assistant Green";
    protected override int DefaultPortCount => 1;
    protected override string DefaultTagsCsv => "homeautomation, homeassistant, smart-home, matter, zigbee, thread";

    public override IEnumerable<DeviceCapability> GetCapabilities(HardwareDevice device) =>
    [
        DeviceCapability.Probe,
        DeviceCapability.WebGui
    ];

    public override IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("api:baseurl", "Home Assistant Base URL", "http://192.168.1.20:8123", GlobalSettingKey: "HomeAssistant:BaseUrl", HelpText: "Home Assistant URL (default port 8123)."),
        new("api:token", "Long-Lived Access Token", "eyJhbGciOiJIUzI1NiIs...", IsSecret: true, GlobalSettingKey: "HomeAssistant:AccessToken", HelpText: "Create under Profile -> Security -> Long-Lived Access Tokens in Home Assistant."),
        new("ssh:username", "SSH Add-on / Host Username (Optional)", "root"),
        new("ssh:password", "SSH Add-on / Host Password (Optional)", "SSH password", IsSecret: true)
    ];
}
