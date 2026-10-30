using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Frigate.Products;

public abstract class NvrProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.Nvr;
    public override string SubCategory => "Network Video Recorders (NVR)";
    public override int DefaultPortCount => 1;
}

public sealed class FrigateNvrApplianceProduct : NvrProductDefinition
{
    public override string ProductId => "frigate-nvr-appliance";
    public override string Vendor => "Frigate";
    public override string Model => "Frigate NVR (Coral / OpenVINO)";
    public override IReadOnlyList<string> DeviceCodes => ["frigate", "frigate-nvr"];
    public override string DefaultTagsCsv => "nvr, frigate, coral, openvino, rtsp, onvif";
    public override string Description => "Real-time local AI object detection NVR for IP cameras with Google Coral TPU or Intel OpenVINO acceleration";
    public override string ProductUrl => "https://frigate.video/";
}

public sealed class ReolinkRln8410NvrProduct : NvrProductDefinition
{
    public override string ProductId => "reolink-rln8-410";
    public override string Vendor => "Reolink";
    public override string Model => "RLN8-410 (8-Channel PoE NVR)";
    public override int DefaultPortCount => 9;
    public override IReadOnlyList<string> DeviceCodes => ["RLN8-410"];
    public override string DefaultTagsCsv => "nvr, reolink, poe, camera";
    public override string Description => "8-Channel PoE Network Video Recorder with 2TB HDD and 8 PoE camera ports";
}

public sealed class UniFiProtectUnvrProduct : NvrProductDefinition
{
    public override string ProductId => "unifi-protect-unvr";
    public override string Vendor => "Ubiquiti UniFi";
    public override string Model => "UniFi Protect Network Video Recorder (UNVR)";
    public override int DefaultPortCount => 2;
    public override IReadOnlyList<string> DeviceCodes => ["UNVR", "UNVR-Pro"];
    public override string DefaultTagsCsv => "nvr, unifi, protect, sfp+";
    public override string Description => "Enterprise 4-bay Network Video Recorder running UniFi Protect with 10G SFP+ and GbE RJ45";
}
