using System;
using NetAdmin.Core.Models;
using Xunit;

namespace NetAdmin.Core.Tests;

public class HardwareDeviceTests
{
    [Fact]
    public void EffectiveManagementUrl_WhenManagementUrlSet_ReturnsManagementUrl()
    {
        var device = new GenericNetworkDevice("id", "name", "1.2.3.4", null, "room", 1, "http://manage.local");
        Assert.Equal("http://manage.local", device.EffectiveManagementUrl);
    }

    [Fact]
    public void EffectiveManagementUrl_WhenNoManagementUrlAndIp_ReturnsHttpUrl_ForNonWanOrFiber()
    {
        var device = new GenericNetworkDevice("id", "name", "10.0.0.5", null, "room");
        Assert.Equal("http://10.0.0.5", device.EffectiveManagementUrl);
    }

    [Fact]
    public void EffectiveManagementUrl_WhenWanCategory_ReturnsEmpty()
    {
        var device = new WanHardwareDevice("id", "wan");
        Assert.Equal(string.Empty, device.EffectiveManagementUrl);
    }

    [Fact]
    public void EffectiveManagementUrl_WhenNoIpOrManagement_ReturnsEmpty()
    {
        var device = new GenericNetworkDevice("id", "name", null, null, "room");
        Assert.Equal(string.Empty, device.EffectiveManagementUrl);
    }
}
