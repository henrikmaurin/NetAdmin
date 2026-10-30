using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Providers.Frigate.Components;

namespace NetAdmin.Providers.Frigate;

public class NvrHardwareUiModule : HardwareUiModuleBase
{
    public override string ModuleId => "nvr-frigate";
    public override string DisplayName => "🎥 NVR";
    public override string Icon => "🎥";
    public override HardwareCategory Category => HardwareCategory.Nvr;
    public override int Priority => 85;

    public override Type? EditorComponentType => typeof(NvrEditor);
    public override Type? CardSummaryComponentType => typeof(NvrCardSummary);

    protected override IReadOnlyList<string> MatchKeywords => ["frigate", "nvr", "coral"];
    protected override string DefaultDeviceName => "Frigate NVR";
    protected override string DefaultRoom => "Server Closet";
    protected override string DefaultVendor => "Frigate";
    protected override string DefaultModel => "Frigate NVR (Coral / OpenVINO)";
    protected override int DefaultPortCount => 1;
    protected override string DefaultTagsCsv => "nvr, frigate, camera, rtsp, ai";

    public override IEnumerable<DeviceCapability> GetCapabilities(HardwareDevice device) =>
    [
        DeviceCapability.Probe,
        DeviceCapability.WebGui
    ];

    public override IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("api:baseurl", "Frigate Base URL", "http://192.168.1.50:5000", GlobalSettingKey: "Frigate:BaseUrl", HelpText: "Frigate HTTP/HTTPS URL (default port 5000 or 8971)."),
        new("api:token", "Frigate Bearer Token / Proxy Token (Optional)", "JWT or proxy token if auth is enabled", IsSecret: true, GlobalSettingKey: "Frigate:ApiToken"),
        new("ssh:username", "Host SSH Username (Optional)", "root"),
        new("ssh:password", "Host SSH Password (Optional)", "SSH password", IsSecret: true)
    ];
}
