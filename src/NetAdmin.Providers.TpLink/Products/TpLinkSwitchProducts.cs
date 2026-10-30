using NetAdmin.Core.Models;

namespace NetAdmin.Providers.TpLink.Products;

public abstract class SwitchProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.Switch;
    public abstract bool IsManaged { get; }
    public virtual bool HasPoe => false;

    public override HardwareDevice ApplyToDevice(HardwareDevice device)
    {
        if (device is SwitchHardwareDevice sw)
        {
            return sw with
            {
                Category = HardwareCategory.Switch,
                Vendor = Vendor,
                SwitchModel = Model,
                Model = Model,
                IsManaged = IsManaged,
                PortCount = DefaultPortCount,
                TagsCsv = string.IsNullOrWhiteSpace(sw.TagsCsv) ? DefaultTagsCsv : sw.TagsCsv
            };
        }

        return new SwitchHardwareDevice(
            Id: device.Id,
            Name: device.Name,
            IpAddress: device.IpAddress,
            MacAddress: device.MacAddress,
            Room: device.Room,
            PortCount: DefaultPortCount,
            IsManaged: IsManaged,
            SwitchModel: Model,
            ManagementUrl: device.ManagementUrl,
            Notes: device.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(device.TagsCsv) ? DefaultTagsCsv : device.TagsCsv,
            Vendor: Vendor);
    }
}

public sealed class TpLinkTlSg108PeProduct : SwitchProductDefinition
{
    public override string ProductId => "tplink-tl-sg108pe";
    public override string Vendor => "TP-Link";
    public override string Model => "TL-SG108PE";
    public override string SubCategory => "Easy Smart PoE Switch";
    public override bool IsManaged => true;
    public override bool HasPoe => true;
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["TL-SG108PE", "SG108PE"];
    public override string DefaultTagsCsv => "switch, tplink, managed, poe, vlan, 8-port";
    public override string Description => "8-Port Gigabit Easy Smart Switch with 4-Port PoE+ (802.1Q VLAN)";
}

public sealed class TpLinkTlSg108EProduct : SwitchProductDefinition
{
    public override string ProductId => "tplink-tl-sg108e";
    public override string Vendor => "TP-Link";
    public override string Model => "TL-SG108E";
    public override string SubCategory => "Easy Smart Switch";
    public override bool IsManaged => true;
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["TL-SG108E", "SG108E"];
    public override string DefaultTagsCsv => "switch, tplink, managed, vlan, 8-port";
    public override string Description => "8-Port Gigabit Easy Smart Switch (802.1Q VLAN)";
}

public sealed class TpLinkTlSg105PeProduct : SwitchProductDefinition
{
    public override string ProductId => "tplink-tl-sg105pe";
    public override string Vendor => "TP-Link";
    public override string Model => "TL-SG105PE";
    public override string SubCategory => "Easy Smart PoE Switch";
    public override bool IsManaged => true;
    public override bool HasPoe => true;
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["TL-SG105PE", "SG105PE"];
    public override string DefaultTagsCsv => "switch, tplink, managed, poe, vlan, 5-port";
    public override string Description => "5-Port Gigabit Easy Smart Switch with 4-Port PoE+";
}

public sealed class TpLinkTlSg108UnmanagedProduct : SwitchProductDefinition
{
    public override string ProductId => "tplink-tl-sg108";
    public override string Vendor => "TP-Link";
    public override string Model => "TL-SG108 (Unmanaged)";
    public override string SubCategory => "Unmanaged Switch";
    public override bool IsManaged => false;
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["TL-SG108", "LS108G"];
    public override string DefaultTagsCsv => "switch, tplink, unmanaged, 8-port";
    public override string Description => "8-Port 10/100/1000Mbps Desktop Switch (Unmanaged)";
}

public sealed class TpLinkTlSg105UnmanagedProduct : SwitchProductDefinition
{
    public override string ProductId => "tplink-tl-sg105";
    public override string Vendor => "TP-Link";
    public override string Model => "TL-SG105 (Unmanaged)";
    public override string SubCategory => "Unmanaged Switch";
    public override bool IsManaged => false;
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["TL-SG105", "LS105G"];
    public override string DefaultTagsCsv => "switch, tplink, unmanaged, 5-port";
    public override string Description => "5-Port 10/100/1000Mbps Desktop Switch (Unmanaged)";
}

public sealed class GenericManagedSwitchProduct : SwitchProductDefinition
{
    public override string ProductId => "generic-switch-managed";
    public override string Vendor => "Generic";
    public override string Model => "Managed / Smart Switch (8p)";
    public override string SubCategory => "Generic Switch";
    public override bool IsManaged => true;
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["GenericManagedSwitch"];
    public override string DefaultTagsCsv => "switch, managed, vlan";
    public override string Description => "Generic Managed / Smart Switch with 802.1Q VLAN support";
}

public sealed class GenericUnmanagedSwitchProduct : SwitchProductDefinition
{
    public override string ProductId => "generic-switch-unmanaged";
    public override string Vendor => "Generic";
    public override string Model => "Unmanaged Switch (8p)";
    public override string SubCategory => "Generic Switch";
    public override bool IsManaged => false;
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["GenericUnmanagedSwitch"];
    public override string DefaultTagsCsv => "switch, unmanaged";
    public override string Description => "Generic Unmanaged Gigabit Switch";
}

// ============================================================================
// TP-Link Tapo Smart Cameras
// ============================================================================

public abstract class TpLinkTapoCameraProductDefinition : HardwareProductDefinition
{
    public override string Vendor => "TP-Link";
    public override HardwareCategory Category => HardwareCategory.Camera;
    public override string SubCategory => "Tapo Smart Cameras";

    public virtual IReadOnlyList<CameraStreamDefinition> DefaultStreams =>
    [
        new("Main Stream (HD)", "/stream1", "RTSP", "2K / High"),
        new("Sub Stream (SD)", "/stream2", "RTSP", "360p / Low")
    ];

    public override HardwareDevice ApplyToDevice(HardwareDevice device)
    {
        if (device is CameraHardwareDevice cam)
        {
            var streams = cam.Streams is { Count: > 0 } ? cam.Streams : DefaultStreams;
            return cam with
            {
                Category = HardwareCategory.Camera,
                Vendor = Vendor,
                Model = Model,
                PortCount = DefaultPortCount,
                RtspPath = streams.FirstOrDefault()?.Url ?? cam.RtspPath,
                Streams = streams,
                TagsCsv = DefaultTagsCsv
            };
        }

        return new CameraHardwareDevice(
            Id: device.Id,
            Name: device.Name,
            IpAddress: device.IpAddress,
            MacAddress: device.MacAddress,
            Room: device.Room,
            Vendor: Vendor,
            Model: Model,
            PortCount: DefaultPortCount,
            RtspPath: DefaultStreams.FirstOrDefault()?.Url,
            Notes: device.Notes,
            TagsCsv: DefaultTagsCsv,
            Streams: DefaultStreams);
    }
}

public sealed class TpLinkTapoC210Product : TpLinkTapoCameraProductDefinition
{
    public override string ProductId => "tplink-tapo-c210";
    public override string Model => "Tapo C210";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["C210", "Tapo C210"];
    public override string DefaultTagsCsv => "camera, tplink, tapo, wifi, ptz, frigate";
    public override string Description => "Pan/Tilt 2K 3MP Indoor Wi-Fi Camera with RTSP & ONVIF";
}

public sealed class TpLinkTapoC220Product : TpLinkTapoCameraProductDefinition
{
    public override string ProductId => "tplink-tapo-c220";
    public override string Model => "Tapo C220";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["C220", "Tapo C220"];
    public override string DefaultTagsCsv => "camera, tplink, tapo, wifi, ptz, 2k-qhd, frigate";
    public override string Description => "Pan/Tilt AI 2K 4MP QHD Indoor Wi-Fi Camera with RTSP & ONVIF";
}

public sealed class TpLinkTapoC320WsProduct : TpLinkTapoCameraProductDefinition
{
    public override string ProductId => "tplink-tapo-c320ws";
    public override string Model => "Tapo C320WS";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["C320WS", "Tapo C320WS"];
    public override string DefaultTagsCsv => "camera, tplink, tapo, outdoor, lan, wifi, frigate";
    public override string Description => "2K 4MP QHD Outdoor Security Camera with Starlight Color Night Vision (Ethernet + Wi-Fi)";
}

public sealed class TpLinkTapoC520WsProduct : TpLinkTapoCameraProductDefinition
{
    public override string ProductId => "tplink-tapo-c520ws";
    public override string Model => "Tapo C520WS";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["C520WS", "Tapo C520WS"];
    public override string DefaultTagsCsv => "camera, tplink, tapo, outdoor, ptz, lan, wifi, frigate";
    public override string Description => "2K QHD Outdoor Pan/Tilt Security Camera with Starlight Night Vision (Ethernet + Wi-Fi)";
}
