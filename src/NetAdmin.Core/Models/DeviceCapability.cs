using System;

namespace NetAdmin.Core.Models;

public enum DeviceCapability
{
    WebGui,
    Ssh,
    Reboot,
    StaticDhcp,
    VlanConfig,
    FirewallConfig,
    ControllerConfig,
    Probe
}

/// <summary>
/// Describes a per-device credential or API configuration field shown on the device details page.
/// Stored encrypted under "device:{DeviceId}:{KeySuffix}" (and optionally synced with GlobalSettingKey).
/// </summary>
public record DeviceCredentialField(
    string KeySuffix,
    string Label,
    string Placeholder = "",
    bool IsSecret = false,
    string? GlobalSettingKey = null,
    string? HelpText = null);

