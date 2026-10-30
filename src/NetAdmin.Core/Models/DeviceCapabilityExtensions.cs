namespace NetAdmin.Core.Models;

public static class DeviceCapabilityExtensions
{
    public static string ToIcon(this DeviceCapability capability) => capability switch
    {
        DeviceCapability.WebGui => "🌐",
        DeviceCapability.Ssh => "🔒",
        DeviceCapability.Reboot => "♻️",
        DeviceCapability.StaticDhcp => "📌",
        DeviceCapability.VlanConfig => "🧩",
        DeviceCapability.FirewallConfig => "🔥",
        DeviceCapability.ControllerConfig => "📡",
        DeviceCapability.Probe => "🔎",
        _ => "?",
    };
}
