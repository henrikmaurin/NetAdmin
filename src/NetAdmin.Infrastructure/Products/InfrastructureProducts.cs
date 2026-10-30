using NetAdmin.Core.Models;

namespace NetAdmin.Infrastructure.Products;

public abstract class CameraProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.Camera;
    public override string SubCategory => "IP & Wi-Fi Cameras";

    public virtual IReadOnlyList<CameraStreamDefinition> DefaultStreams => [];

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

public abstract class ReolinkCameraProductDefinition : CameraProductDefinition
{
    public override string Vendor => "Reolink";
    public override string ProductUrl => "https://store.reolink.com/";

    public override IReadOnlyList<CameraStreamDefinition> DefaultStreams =>
    [
        new("Main Stream", "/h264Preview_01_main", "RTSP", "4K / High"),
        new("Sub Stream", "/h264Preview_01_sub", "RTSP", "640x360 / Fluent")
    ];
}

public sealed class ReolinkRlc810AProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-rlc-810a";
    public override string Model => "RLC-810A";
    public override string SubCategory => "PoE IP Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["RLC-810A"];
    public override string DefaultTagsCsv => "camera, reolink, poe, 4k, frigate";
    public override string Description => "4K 8MP Smart PoE Bullet IP Camera with Person/Vehicle Detection (RTSP/ONVIF)";
}

public sealed class ReolinkRlc811AProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-rlc-811a";
    public override string Model => "RLC-811A";
    public override string SubCategory => "PoE IP Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["RLC-811A"];
    public override string DefaultTagsCsv => "camera, reolink, poe, 4k, zoom, spotlight, frigate";
    public override string Description => "4K 8MP Smart PoE Spotlight Camera with 5X Optical Zoom & Two-Way Audio";
}

public sealed class ReolinkRlc820AProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-rlc-820a";
    public override string Model => "RLC-820A";
    public override string SubCategory => "PoE IP Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["RLC-820A"];
    public override string DefaultTagsCsv => "camera, reolink, dome, poe, 4k, frigate";
    public override string Description => "4K 8MP Smart PoE Turret/Dome IP Camera (RTSP/ONVIF)";
}

public sealed class ReolinkRlc1240AProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-rlc-1240a";
    public override string Model => "RLC-1240A";
    public override string SubCategory => "PoE IP Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["RLC-1240A"];
    public override string DefaultTagsCsv => "camera, reolink, dome, poe, 12mp, ik10, frigate";
    public override string Description => "12MP Ultra HD IK10 Vandal-Proof PoE Vandal Dome Camera";
}

public sealed class ReolinkCx820Product : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-cx820";
    public override string Model => "CX820";
    public override string SubCategory => "ColorX Night Vision Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["CX820"];
    public override string DefaultTagsCsv => "camera, reolink, colorx, poe, 4k, frigate";
    public override string Description => "4K ColorX True Full-Color Night Vision PoE Turret Camera with F1.0 Aperture";
}

public sealed class ReolinkDuo3PoeProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-duo-3-poe";
    public override string Model => "Duo 3 PoE";
    public override string SubCategory => "Dual-Lens Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Duo 3 PoE", "Reolink Duo 3 PoE"];
    public override string DefaultTagsCsv => "camera, reolink, dual-lens, 180-degree, poe, 16mp, frigate";
    public override string Description => "16MP UHD Dual-Lens PoE Camera with 180° Panorama View";
}

public sealed class ReolinkTrackMixPoeProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-trackmix-poe";
    public override string Model => "TrackMix PoE";
    public override string SubCategory => "Dual-Lens PTZ Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["TrackMix PoE"];
    public override string DefaultTagsCsv => "camera, reolink, dual-lens, ptz, poe, 4k, frigate";
    public override string Description => "4K Dual-Lens PTZ PoE Camera with Auto-Tracking and Dual View";
    public override IReadOnlyList<CameraStreamDefinition> DefaultStreams =>
    [
        new("Main Stream (Wide)", "/h264Preview_01_main", "RTSP", "4K"),
        new("Sub Stream (Wide)", "/h264Preview_01_sub", "RTSP", "Fluent"),
        new("Telephoto Stream", "/h264Preview_02_main", "RTSP", "1080p Zoom")
    ];
}

public sealed class ReolinkDoorbellPoeProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-doorbell-poe";
    public override string Model => "Video Doorbell PoE";
    public override string SubCategory => "Video Doorbells";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Video Doorbell PoE", "DB_560612V1"];
    public override string DefaultTagsCsv => "camera, reolink, doorbell, poe, frigate";
    public override string Description => "2K+ 5MP Smart PoE Video Doorbell with Two-Way Audio & RTSP";
}

public sealed class ReolinkDoorbellWifiProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-doorbell-wifi";
    public override string Model => "Video Doorbell Wi-Fi";
    public override string SubCategory => "Video Doorbells";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Video Doorbell WiFi"];
    public override string DefaultTagsCsv => "camera, reolink, doorbell, wifi, frigate";
    public override string Description => "2K+ 5MP Dual-Band Wi-Fi Video Doorbell with RTSP/ONVIF";
}

public sealed class ReolinkE1ZoomProduct : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-e1-zoom";
    public override string Model => "E1 Zoom";
    public override string SubCategory => "Wi-Fi Indoor Cameras";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["E1 Zoom"];
    public override string DefaultTagsCsv => "camera, reolink, indoor, wifi, ptz, frigate";
    public override string Description => "5MP Indoor Pan/Tilt Wi-Fi Camera with 3X Optical Zoom";
}

public sealed class ReolinkWp320Product : ReolinkCameraProductDefinition
{
    public override string ProductId => "reolink-wp320";
    public override string Model => "WP320";
    public override string SubCategory => "IP & Wi-Fi Cameras";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["WP320", "Reolink WP320"];
    public override string DefaultTagsCsv => "camera, reolink, wp320, rtsp, frigate";
    public override string Description => "Reolink WP320 Smart Camera with RTSP/ONVIF support";
}

public sealed class GenericPoeCameraProduct : CameraProductDefinition
{
    public override string ProductId => "generic-camera-poe";
    public override string Vendor => "Generic";
    public override string Model => "PoE IP Camera (RTSP/ONVIF)";
    public override string SubCategory => "Generic Camera";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericPoeCamera"];
    public override string DefaultTagsCsv => "camera, poe, rtsp, onvif, frigate";
    public override string Description => "Generic wired PoE IP camera with 1 RJ45 port";
    public override IReadOnlyList<CameraStreamDefinition> DefaultStreams =>
    [
        new("Main Stream", "/stream1", "RTSP", "Main"),
        new("Sub Stream", "/stream2", "RTSP", "Sub")
    ];
}

public sealed class GenericWifiCameraProduct : CameraProductDefinition
{
    public override string ProductId => "generic-camera-wifi";
    public override string Vendor => "Generic";
    public override string Model => "Wi-Fi Camera (RTSP)";
    public override string SubCategory => "Generic Camera";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["GenericWifiCamera"];
    public override string DefaultTagsCsv => "camera, wifi, rtsp, frigate";
    public override string Description => "Generic wireless Wi-Fi camera (0 physical LAN ports)";
    public override IReadOnlyList<CameraStreamDefinition> DefaultStreams =>
    [
        new("Main Stream", "/stream1", "RTSP", "Main")
    ];
}

// ============================================================================
// IoT Devices (Generic)
// ============================================================================

public abstract class IoTProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.IoT;
}

public sealed class GenericWifiIoTDeviceProduct : IoTProductDefinition
{
    public override string ProductId => "generic-iot-wifi";
    public override string Vendor => "Generic";
    public override string Model => "Wi-Fi IoT Device";
    public override string SubCategory => "Generic IoT";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["GenericWifiIoT"];
    public override string DefaultTagsCsv => "iot, wifi, sensor";
    public override string Description => "Generic wireless IoT device or sensor (Wi-Fi, 0 LAN ports)";
}

public sealed class GenericWiredIoTDeviceProduct : IoTProductDefinition
{
    public override string ProductId => "generic-iot-lan";
    public override string Vendor => "Generic";
    public override string Model => "Wired LAN IoT Device";
    public override string SubCategory => "Generic IoT";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericLanIoT"];
    public override string DefaultTagsCsv => "iot, lan";
    public override string Description => "Generic wired IoT device with 1 RJ45 LAN port";
}

// ============================================================================
// Phones, Tablets, TVs, Media Players & Computers
// ============================================================================

public sealed class GenericPhoneProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-phone";
    public override string Vendor => "Generic";
    public override string Model => "Smartphone";
    public override HardwareCategory Category => HardwareCategory.Phone;
    public override string SubCategory => "Smartphones";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["GenericPhone"];
    public override string DefaultTagsCsv => "phone, wifi";
    public override string Description => "Generic mobile phone / smartphone on Wi-Fi";
}

public sealed class GenericTabletProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-tablet";
    public override string Vendor => "Generic";
    public override string Model => "Tablet";
    public override HardwareCategory Category => HardwareCategory.Tablet;
    public override string SubCategory => "Tablets";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["GenericTablet"];
    public override string DefaultTagsCsv => "tablet, wifi";
    public override string Description => "Generic tablet / wall dashboard panel";
}

public sealed class LgOledSmartTvProduct : HardwareProductDefinition
{
    public override string ProductId => "lg-oled-smart-tv";
    public override string Vendor => "LG";
    public override string Model => "OLED / QNED webOS Smart TV";
    public override HardwareCategory Category => HardwareCategory.Tv;
    public override string SubCategory => "Smart TVs";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["LGwebOSTV", "OLED", "LG-TV"];
    public override string DefaultTagsCsv => "tv, lg, webos, lan, wifi";
    public override string Description => "LG OLED / QNED Smart TV running webOS with RJ45 LAN & Wi-Fi";
}

public sealed class GenericSmartTvProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-smart-tv";
    public override string Vendor => "Generic";
    public override string Model => "Smart TV";
    public override HardwareCategory Category => HardwareCategory.Tv;
    public override string SubCategory => "Smart TVs";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericTV"];
    public override string DefaultTagsCsv => "tv, media, lan, wifi";
    public override string Description => "Generic network-connected Smart TV";
}

public sealed class NvidiaShieldTvProProduct : HardwareProductDefinition
{
    public override string ProductId => "nvidia-shield-tv-pro";
    public override string Vendor => "NVIDIA";
    public override string Model => "SHIELD Android TV Pro (4K HDR)";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["SHIELD", "SHIELD-Android-TV"];
    public override string DefaultTagsCsv => "mediaplayer, nvidia, shield, android-tv, plex, lan";
    public override string Description => "NVIDIA SHIELD TV Pro 4K HDR streaming media player with Gigabit Ethernet & Plex Media Server";
}

public sealed class GenericMediaPlayerProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-media-player";
    public override string Vendor => "Generic";
    public override string Model => "Media Player / Streaming Box";
    public override HardwareCategory Category => HardwareCategory.MediaPlayer;
    public override string SubCategory => "Streaming Media Players";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericMediaPlayer"];
    public override string DefaultTagsCsv => "mediaplayer, streaming, media";
    public override string Description => "Generic network media player, streaming box, or audio streamer";
}

public sealed class SonosEra100SmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "sonos-era-100";
    public override string Vendor => "Sonos";
    public override string Model => "Era 100 / Era 300";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["Sonos-Era-100", "Sonos-Era-300", "Sonos-One"];
    public override string DefaultTagsCsv => "smartspeaker, sonos, airplay2, wifi, audio";
    public override string Description => "Sonos wireless smart speaker with Wi-Fi, Bluetooth & AirPlay 2";
}

public sealed class GenericSmartSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-smart-speaker";
    public override string Vendor => "Generic";
    public override string Model => "Smart Speaker";
    public override HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public override string SubCategory => "Smart Speakers";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["GenericSmartSpeaker"];
    public override string DefaultTagsCsv => "smartspeaker, audio, wifi";
    public override string Description => "Generic voice-enabled or multi-room smart speaker";
}

public sealed class SonosArcSoundbarProduct : HardwareProductDefinition
{
    public override string ProductId => "sonos-arc-beam-soundbar";
    public override string Vendor => "Sonos";
    public override string Model => "Arc / Arc Ultra / Beam (Gen 2)";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Soundbars";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["Sonos-Arc", "Sonos-Beam", "Sonos-Ray"];
    public override string DefaultTagsCsv => "soundbar, sonos, earc, dolby-atmos, lan, wifi";
    public override string Description => "Sonos Dolby Atmos soundbar with HDMI eARC, Ethernet & Wi-Fi";
}

public sealed class GenericSoundbarSpeakerProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-soundbar-speaker";
    public override string Vendor => "Generic";
    public override string Model => "Soundbar / Networked Speaker";
    public override HardwareCategory Category => HardwareCategory.Soundbar;
    public override string SubCategory => "Soundbars & Connected Speakers";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericSoundbar", "ConnectedSpeaker"];
    public override string DefaultTagsCsv => "soundbar, speaker, audio";
    public override string Description => "Soundbar, AV receiver, or connected non-smart speaker (Wi-Fi / Ethernet / AirPlay / Spotify Connect)";
}

public sealed class GenericPrinterProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-network-printer";
    public override string Vendor => "Generic";
    public override string Model => "Network Printer / Multifunction Scanner";
    public override HardwareCategory Category => HardwareCategory.Printer;
    public override string SubCategory => "Printers & Scanners";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericPrinter", "Printer"];
    public override string DefaultTagsCsv => "printer, scanner, lan, wifi";
    public override string Description => "Generic Wi-Fi or Ethernet network printer / multifunction scanner";
}

public sealed class GenericComputerProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-computer";
    public override string Vendor => "Generic";
    public override string Model => "Desktop PC / Laptop";
    public override HardwareCategory Category => HardwareCategory.Computer;
    public override string SubCategory => "Computers & Workstations";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericComputer", "PC"];
    public override string DefaultTagsCsv => "computer, workstation";
    public override string Description => "Generic Windows / Linux desktop or laptop computer";
}

// ============================================================================
// Other Infrastructure Categories
// ============================================================================

public sealed class GenericPatchPanel8PortProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-patchpanel-8p";
    public override string Vendor => "Generic";
    public override string Model => "8-Port Cat6 Patch Panel";
    public override HardwareCategory Category => HardwareCategory.PatchPanel;
    public override string SubCategory => "Patch Panels";
    public override int DefaultPortCount => 8;
    public override IReadOnlyList<string> DeviceCodes => ["PatchPanel8"];
    public override string DefaultTagsCsv => "patchpanel, cat6, 8-port";
    public override string Description => "Generic 8-port RJ45 Cat6 patch panel";
}

public sealed class GenericPatchPanel24PortProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-patchpanel-24p";
    public override string Vendor => "Generic";
    public override string Model => "24-Port Cat6 Patch Panel (19\" Rack)";
    public override HardwareCategory Category => HardwareCategory.PatchPanel;
    public override string SubCategory => "Patch Panels";
    public override int DefaultPortCount => 24;
    public override IReadOnlyList<string> DeviceCodes => ["PatchPanel24"];
    public override string DefaultTagsCsv => "patchpanel, cat6, 24-port, rack";
    public override string Description => "Generic 24-port 1U RJ45 Cat6 patch panel";
}

public sealed class GenericServerNasProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-server-proxmox";
    public override string Vendor => "Generic";
    public override string Model => "Server / Proxmox / NAS";
    public override HardwareCategory Category => HardwareCategory.Server;
    public override string SubCategory => "Servers & Virtualization";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericServer", "Proxmox"];
    public override string DefaultTagsCsv => "server, proxmox, nas, linux";
    public override string Description => "Generic physical server, Mini-PC (N100/N150) or NAS";
}

public sealed class GenericFiberConverterProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-fiber-ont";
    public override string Vendor => "Generic";
    public override string Model => "Fiber Converter / ONT (CPE)";
    public override HardwareCategory Category => HardwareCategory.FiberConverter;
    public override string SubCategory => "Fiber & WAN";
    public override int DefaultPortCount => 5;
    public override IReadOnlyList<string> DeviceCodes => ["GenericONT", "CPE"];
    public override string DefaultTagsCsv => "fiber, ont, cpe, wan";
    public override string Description => "Generic fiber media converter / ONT for incoming fiber connection";
}

public sealed class GenericWanEndpointProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-wan-isp";
    public override string Vendor => "Generic";
    public override string Model => "Internet / WAN Connection (ISP)";
    public override HardwareCategory Category => HardwareCategory.WanEndpoint;
    public override string SubCategory => "Fiber & WAN";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericWAN", "ISP"];
    public override string DefaultTagsCsv => "wan, internet, isp";
    public override string Description => "Generic external internet service provider / WAN endpoint";
}

public sealed class GenericClientNetworkDeviceProduct : HardwareProductDefinition
{
    public override string ProductId => "generic-network-device";
    public override string Vendor => "Generic";
    public override string Model => "Network Client Device";
    public override HardwareCategory Category => HardwareCategory.Generic;
    public override string SubCategory => "Generic Equipment";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["GenericDevice"];
    public override string DefaultTagsCsv => "client, generic";
    public override string Description => "Generic network-connected client device";
}
