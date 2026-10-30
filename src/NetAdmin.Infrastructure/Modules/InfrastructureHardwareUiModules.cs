using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Components;

namespace NetAdmin.Infrastructure.Modules;

public class PatchPanelHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "patchpanel";
    public string DisplayName => "🔌 Patch Panel";
    public string Icon => "🔌";
    public HardwareCategory Category => HardwareCategory.PatchPanel;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(PatchPanelEditor);
    public Type? CardSummaryComponentType => typeof(PatchPanelCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is PatchPanelHardwareDevice || device.Category == HardwareCategory.PatchPanel;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.PatchPanel;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new PatchPanelHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Patch Panel" : name,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 8,
            Notes: "",
            TagsCsv: "patchpanel, cat6");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is PatchPanelHardwareDevice pp)
        {
            return pp with { Category = HardwareCategory.PatchPanel };
        }

        return new PatchPanelHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            Room: current.Room,
            PortCount: current.PortCount <= 1 ? 8 : current.PortCount,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "patchpanel" : current.TagsCsv);
    }
}

public class CameraHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "camera";
    public string DisplayName => "📷 Camera";
    public string Icon => "📷";
    public HardwareCategory Category => HardwareCategory.Camera;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(CameraEditor);
    public Type? CardSummaryComponentType => typeof(CameraCardSummary);

    public bool CanHandle(HardwareDevice device) =>
        device is CameraHardwareDevice || device.Category == HardwareCategory.Camera;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Camera;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new CameraHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Camera" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            Vendor: "Reolink",
            Model: "RLC-810A",
            PortCount: 1,
            RtspPath: null,
            Notes: "",
            TagsCsv: "camera, frigate");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is CameraHardwareDevice cam)
        {
            return cam with { Category = HardwareCategory.Camera };
        }

        return new CameraHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            Vendor: string.IsNullOrWhiteSpace(current.Vendor) ? "Reolink" : current.Vendor,
            Model: current.Model,
            PortCount: current.PortCount,
            RtspPath: null,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "camera, frigate" : current.TagsCsv)
        {
            ManagementUrl = current.ManagementUrl
        };
    }

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("rtsp:username", "Camera Username (RTSP / ONVIF / HTTP)", "admin"),
        new("rtsp:password", "Camera Password", "Camera password", IsSecret: true),
        new("http:token", "HTTP API Token / Key (Optional)", "API token", IsSecret: true)
    ];
}

public class ServerHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "server";
    public string DisplayName => "🖥️ Server";
    public string Icon => "🖥️";
    public HardwareCategory Category => HardwareCategory.Server;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Server;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Server;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Server" : name,
            Category: HardwareCategory.Server,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "server, proxmox",
            Vendor: "Generic",
            Model: "Server / Mini-PC / NAS");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Server,
            PortCount = current.PortCount <= 0 ? 1 : current.PortCount
        };

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) =>
    [
        new("ssh:username", "SSH Username", "root"),
        new("ssh:password", "SSH Password / Private Key", "SSH password or key", IsSecret: true),
        new("http:token", "API Token (e.g. Proxmox / TrueNAS / Docker)", "Bearer or API token", IsSecret: true)
    ];
}

public class FiberConverterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "fiberconverter";
    public string DisplayName => "🔦 Fiber Converter";
    public string Icon => "🔦";
    public HardwareCategory Category => HardwareCategory.FiberConverter;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(FiberConverterEditor);

    public bool CanHandle(HardwareDevice device) =>
        device is FiberConverterHardwareDevice || device.Category == HardwareCategory.FiberConverter;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.FiberConverter;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new FiberConverterHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "Fiber Converter" : name,
            Room: string.IsNullOrWhiteSpace(room) ? "Server Closet" : room,
            PortCount: 5,
            Vendor: "City Network",
            ConverterModel: "CPE / ONT",
            Notes: "",
            TagsCsv: "wan, fiber, cpe");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is FiberConverterHardwareDevice fc)
        {
            return fc with { Category = HardwareCategory.FiberConverter, Model = fc.ConverterModel };
        }

        return new FiberConverterHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            Room: current.Room,
            PortCount: current.PortCount <= 1 ? 5 : current.PortCount,
            Vendor: string.IsNullOrWhiteSpace(current.Vendor) ? "City Network" : current.Vendor,
            ConverterModel: string.IsNullOrWhiteSpace(current.Model) ? "CPE / ONT" : current.Model,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "wan, fiber, cpe" : current.TagsCsv);
    }
}

public class WanEndpointHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "wan";
    public string DisplayName => "🌐 WAN Endpoint";
    public string Icon => "🌐";
    public HardwareCategory Category => HardwareCategory.WanEndpoint;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(WanEndpointEditor);

    public bool CanHandle(HardwareDevice device) =>
        device is WanHardwareDevice || device.Category == HardwareCategory.WanEndpoint;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.WanEndpoint;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new WanHardwareDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "Internet / WAN" : name,
            PublicIpAddress: ipAddress,
            IspName: "ISP",
            Notes: "Incoming WAN connection",
            TagsCsv: "internet, wan, isp");

    public HardwareDevice EnsureDeviceType(HardwareDevice current)
    {
        if (current is WanHardwareDevice wan)
        {
            return wan with { Category = HardwareCategory.WanEndpoint, Vendor = wan.IspName, IpAddress = wan.PublicIpAddress };
        }

        return new WanHardwareDevice(
            Id: current.Id,
            Name: current.Name,
            PublicIpAddress: current.IpAddress,
            IspName: string.IsNullOrWhiteSpace(current.Vendor) ? "ISP" : current.Vendor,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "internet, wan, isp" : current.TagsCsv);
    }
}

public class GenericIoTHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "iot-generic";
    public string DisplayName => "⚡ IoT";
    public string Icon => "⚡";
    public HardwareCategory Category => HardwareCategory.IoT;
    public int Priority => 10;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.IoT && device is not ShellyHardwareDevice;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.IoT;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New IoT Device" : name,
            Category: HardwareCategory.IoT,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "iot",
            Vendor: "",
            Model: "");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        new(
            Id: current.Id,
            Name: current.Name,
            Category: HardwareCategory.IoT,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            PortCount: current.PortCount,
            ManagementUrl: current.ManagementUrl,
            Notes: current.Notes,
            TagsCsv: string.IsNullOrWhiteSpace(current.TagsCsv) ? "iot" : current.TagsCsv,
            Vendor: current.Vendor,
            Model: current.Model);

    public IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice current) =>
    [
        new("http:token", "API Key / Local Token", "Optional API token", IsSecret: true),
        new("http:password", "Device Password", "Optional password", IsSecret: true)
    ];
}

public class PhoneHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "phone";
    public string DisplayName => "📱 Phone";
    public string Icon => "📱";
    public HardwareCategory Category => HardwareCategory.Phone;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Phone;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Phone;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Phone" : name,
            Category: HardwareCategory.Phone,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "phone, wifi, mobile",
            Vendor: "Apple",
            Model: "iPhone");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Phone,
            PortCount = 0,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "phone, wifi, mobile" : current.TagsCsv
        };
}

public class TabletHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "tablet";
    public string DisplayName => "📲 Tablet";
    public string Icon => "📲";
    public HardwareCategory Category => HardwareCategory.Tablet;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Tablet;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Tablet;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Tablet" : name,
            Category: HardwareCategory.Tablet,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "tablet, wifi",
            Vendor: "Apple",
            Model: "iPad");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Tablet,
            PortCount = 0,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "tablet, wifi" : current.TagsCsv
        };
}

public class SmartWatchHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "smartwatch";
    public string DisplayName => "⌚ Smart Watch";
    public string Icon => "⌚";
    public HardwareCategory Category => HardwareCategory.SmartWatch;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.SmartWatch;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.SmartWatch ||
        (vendor?.Contains("XiaoMi", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (vendor?.Contains("Apple", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (vendor?.Contains("Samsung", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (vendor?.Contains("Google", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Smart Watch" : name,
            Category: HardwareCategory.SmartWatch,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "watch, wearable",
            Vendor: "XiaoMi",
            Model: "Watch");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.SmartWatch,
            PortCount = 0,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "watch, wearable" : current.TagsCsv
        };
}

public class TvHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "tv";
    public string DisplayName => "📺 TV";
    public string Icon => "📺";
    public HardwareCategory Category => HardwareCategory.Tv;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Tv;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Tv ||
        (hostname?.Contains("bravia", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("lgwebostv", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Smart TV" : name,
            Category: HardwareCategory.Tv,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "tv, media",
            Vendor: "Samsung",
            Model: "Smart TV");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Tv,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "tv, media" : current.TagsCsv
        };
}

public class MediaPlayerHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "mediaplayer";
    public string DisplayName => "🎬 Media Player";
    public string Icon => "🎬";
    public HardwareCategory Category => HardwareCategory.MediaPlayer;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.MediaPlayer;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.MediaPlayer ||
        (hostname?.Contains("appletv", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("apple-tv", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("chromecast", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("mibox", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("shield", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Media Player" : name,
            Category: HardwareCategory.MediaPlayer,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "mediaplayer, streaming, media",
            Vendor: "Apple",
            Model: "Apple TV 4K");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.MediaPlayer,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "mediaplayer, streaming, media" : current.TagsCsv
        };
}

public class ComputerHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "computer";
    public string DisplayName => "💻 Computer";
    public string Icon => "💻";
    public HardwareCategory Category => HardwareCategory.Computer;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Computer;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Computer ||
        (vendor?.Equals("Dell", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("xps", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("dell", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("macbook", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("imac", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("mac-mini", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("desktop-", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("laptop-", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Computer" : name,
            Category: HardwareCategory.Computer,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "computer, workstation",
            Vendor: "Dell",
            Model: "XPS Laptop (13 / 14 / 15 / 16 / 17)");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Computer,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "computer, workstation" : current.TagsCsv
        };
}

public class SmartSpeakerHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "smartspeaker";
    public string DisplayName => "🔊 Smart Speaker";
    public string Icon => "🔊";
    public HardwareCategory Category => HardwareCategory.SmartSpeaker;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.SmartSpeaker;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.SmartSpeaker ||
        (hostname?.Contains("homepod", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("nest-audio", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("nest-mini", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("echo-", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Smart Speaker" : name,
            Category: HardwareCategory.SmartSpeaker,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "smartspeaker, audio, wifi",
            Vendor: "Apple",
            Model: "HomePod mini");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.SmartSpeaker,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "smartspeaker, audio, wifi" : current.TagsCsv
        };
}

public class SoundbarHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "soundbar";
    public string DisplayName => "🔉 Soundbar";
    public string Icon => "🔉";
    public HardwareCategory Category => HardwareCategory.Soundbar;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Soundbar;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Soundbar ||
        (vendor?.Contains("Yamaha", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("soundbar", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("yas-", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("yas109", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("yamaha", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("sonos", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("symfonisk", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Soundbar / Speaker" : name,
            Category: HardwareCategory.Soundbar,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "soundbar, speaker, audio",
            Vendor: "Yamaha",
            Model: "YAS-109 Soundbar");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Soundbar,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "soundbar, speaker, audio" : current.TagsCsv
        };
}

public class PrinterHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "printer";
    public string DisplayName => "🖨️ Printer";
    public string Icon => "🖨️";
    public HardwareCategory Category => HardwareCategory.Printer;
    public int Priority => 50;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Printer;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Printer ||
        (vendor?.Contains("Epson", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("epson", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("et-2650", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("et2650", StringComparison.OrdinalIgnoreCase) ?? false) ||
        (hostname?.Contains("printer", StringComparison.OrdinalIgnoreCase) ?? false);

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Printer" : name,
            Category: HardwareCategory.Printer,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 0,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "printer, scanner, wifi",
            Vendor: "Epson",
            Model: "EcoTank ET-2650");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        current with
        {
            Category = HardwareCategory.Printer,
            TagsCsv = string.IsNullOrWhiteSpace(current.TagsCsv) ? "printer, scanner, wifi" : current.TagsCsv
        };
}

public class GenericHardwareUiModule : IHardwareUiModule
{
    public string ModuleId => "generic";
    public string DisplayName => "🔌 Generic";
    public string Icon => "🔌";
    public HardwareCategory Category => HardwareCategory.Generic;
    public int Priority => 0;

    public Type? EditorComponentType => typeof(IoTDeviceEditor);

    public bool CanHandle(HardwareDevice device) =>
        device.Category == HardwareCategory.Generic;

    public bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null) =>
        category == HardwareCategory.Generic;

    public HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "") =>
        new GenericNetworkDevice(
            Id: id,
            Name: string.IsNullOrWhiteSpace(name) ? "New Device" : name,
            IpAddress: ipAddress,
            MacAddress: macAddress,
            Room: room,
            PortCount: 1,
            ManagementUrl: null,
            Notes: "",
            TagsCsv: "");

    public HardwareDevice EnsureDeviceType(HardwareDevice current) =>
        new GenericNetworkDevice(
            Id: current.Id,
            Name: current.Name,
            IpAddress: current.IpAddress,
            MacAddress: current.MacAddress,
            Room: current.Room,
            PortCount: current.PortCount,
            ManagementUrl: current.ManagementUrl,
            Notes: current.Notes,
            TagsCsv: current.TagsCsv,
            Vendor: current.Vendor,
            Model: current.Model);
}

public static class InfrastructureHardwareModuleExtensions
{
    public static IServiceCollection AddInfrastructureHardwareModules(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, PatchPanelHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, CameraHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, ServerHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, FiberConverterHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, WanEndpointHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, GenericIoTHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, PhoneHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, TabletHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, SmartWatchHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, TvHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, MediaPlayerHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, SmartSpeakerHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, SoundbarHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, PrinterHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, ComputerHardwareUiModule>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHardwareUiModule, GenericHardwareUiModule>());
        services.TryAddSingleton<HardwareTypeRegistry>();
        services.TryAddSingleton<ProductCatalogRegistry>();

        RegisterAssemblyHardwareTypes(services, typeof(InfrastructureHardwareModuleExtensions).Assembly);

        return services;
    }

    /// <summary>
    /// Discovers all NetAdmin.Providers.*.dll assemblies via reflection, invokes their INetAdminProviderPlugin
    /// implementations, and registers all IHardwareUiModule and HardwareProductDefinition classes automatically.
    /// </summary>
    public static IServiceCollection AddNetAdminProviderPlugins(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton<HardwareTypeRegistry>();
        services.TryAddSingleton<ProductCatalogRegistry>();

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.GetName().Name))
            .ToDictionary(a => a.GetName().Name!, StringComparer.OrdinalIgnoreCase);

        if (Directory.Exists(AppContext.BaseDirectory))
        {
            foreach (var dllPath in Directory.EnumerateFiles(AppContext.BaseDirectory, "NetAdmin.Providers.*.dll"))
            {
                var assemblyName = Path.GetFileNameWithoutExtension(dllPath);
                if (!loadedAssemblies.ContainsKey(assemblyName))
                {
                    try
                    {
                        var asm = Assembly.LoadFrom(dllPath);
                        loadedAssemblies[asm.GetName().Name ?? assemblyName] = asm;
                    }
                    catch
                    {
                        // Ignore assemblies that cannot be loaded
                    }
                }
            }
        }

        var providerAssemblies = loadedAssemblies.Values
            .Where(a => a.GetName().Name?.StartsWith("NetAdmin.Providers.", StringComparison.OrdinalIgnoreCase) == true)
            .OrderBy(a => a.GetName().Name)
            .ToList();

        foreach (var assembly in providerAssemblies)
        {
            var pluginTypes = GetLoadableTypes(assembly)
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(INetAdminProviderPlugin).IsAssignableFrom(t));

            foreach (var pluginType in pluginTypes)
            {
                if (Activator.CreateInstance(pluginType) is INetAdminProviderPlugin plugin)
                {
                    plugin.RegisterServices(services, configuration);
                }
            }

            RegisterAssemblyHardwareTypes(services, assembly);
        }

        return services;
    }

    private static void RegisterAssemblyHardwareTypes(IServiceCollection services, Assembly assembly)
    {
        foreach (var type in GetLoadableTypes(assembly))
        {
            if (type is not { IsClass: true, IsAbstract: false })
                continue;

            if (typeof(HardwareProductDefinition).IsAssignableFrom(type))
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(HardwareProductDefinition), type));
            }

            if (typeof(IHardwareUiModule).IsAssignableFrom(type))
            {
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHardwareUiModule), type));
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}
