using NetAdmin.Core.Models;
using Xunit;

namespace NetAdmin.Core.Tests;

public class ServiceTypeExtensionsTests
{
    [Theory]
    [InlineData(ServiceType.Other, "Other")]
    [InlineData(ServiceType.HomeAutomation, "Home Automation")]
    [InlineData(ServiceType.Nvr, "NVR")]
    [InlineData(ServiceType.Monitoring, "Monitoring")]
    public void ToDisplayName_ReturnsExpected(ServiceType t, string expected)
    {
        Assert.Equal(expected, t.ToDisplayName());
    }
}
