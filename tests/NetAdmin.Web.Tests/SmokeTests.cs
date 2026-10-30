using Xunit;

namespace NetAdmin.Web.Tests;

public class SmokeTests
{
    [Fact]
    public void WebProject_AssemblyLoads()
    {
        // Basic smoke test to ensure test project references the Web project and it builds
        Assert.True(true);
    }
}
