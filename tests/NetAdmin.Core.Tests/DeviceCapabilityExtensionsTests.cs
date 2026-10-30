using NetAdmin.Core.Models;
using Xunit;

namespace NetAdmin.Core.Tests;

public class DeviceCapabilityExtensionsTests
{
    [Theory]
    [InlineData(DeviceCapability.WebGui, "🌐")]
    [InlineData(DeviceCapability.Ssh, "🔒")]
    [InlineData(DeviceCapability.Reboot, "♻️")]
    [InlineData(DeviceCapability.StaticDhcp, "📌")]
    [InlineData(DeviceCapability.VlanConfig, "🧩")]
    [InlineData(DeviceCapability.FirewallConfig, "🔥")]
    [InlineData(DeviceCapability.ControllerConfig, "📡")]
    [InlineData(DeviceCapability.Probe, "🔎")]
    public void ToIcon_ReturnsExpectedIcon(DeviceCapability cap, string expected)
    {
        Assert.Equal(expected, cap.ToIcon());
    }
}
