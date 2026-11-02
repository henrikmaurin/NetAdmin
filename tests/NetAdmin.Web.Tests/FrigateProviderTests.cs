using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NetAdmin.Core.Models;
using NetAdmin.Core.Services;
using NetAdmin.Providers.Frigate;
using Xunit;

namespace NetAdmin.Web.Tests;

public sealed class FrigateProviderTests
{
    [Fact]
    public async Task GetCameraDefinitionsAsync_ReturnsEveryConfiguredFfmpegInput()
    {
        const string config = """
            {
              "cameras": {
                "front_door": {
                  "ffmpeg": {
                    "inputs": [
                      { "path": "rtsp://camera.local/main", "roles": ["detect", "record"] },
                      { "path": "rtsp://camera.local/sub", "roles": ["detect"] }
                    ]
                  }
                },
                "porch": { "detect": { "enabled": true } }
              }
            }
            """;
        using var httpClient = new HttpClient(new StubHttpMessageHandler(config));
        var provider = new FrigateProvider(httpClient, new StubEncryptedSettingsService());

        var definitions = await provider.GetCameraDefinitionsAsync(frigateBaseUrl: "http://frigate.test:5000");

        Assert.Equal(2, definitions.Count);
        var frontDoor = Assert.Single(definitions, definition => definition.Name == "front_door");
        Assert.Collection(frontDoor.Streams,
            stream =>
            {
                Assert.Equal("rtsp://camera.local/main", stream.Url);
                Assert.Equal("RTSP", stream.Protocol);
                Assert.Equal("Stream 1 (detect, record)", stream.Name);
            },
            stream =>
            {
                Assert.Equal("rtsp://camera.local/sub", stream.Url);
                Assert.Equal("Stream 2 (detect)", stream.Name);
            });
        Assert.Empty(definitions.Single(definition => definition.Name == "porch").Streams);

        var sources = await provider.GetCameraSourcesAsync(frigateBaseUrl: "http://frigate.test:5000");
        Assert.Equal("rtsp://camera.local/main", sources["front_door"]);
    }

    private sealed class StubHttpMessageHandler(string content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            });
    }

    private sealed class StubEncryptedSettingsService : IEncryptedSettingsService
    {
        public Task<string?> GetSettingAsync(string key, string? fallbackConfigPath = null, CancellationToken ct = default) =>
            Task.FromResult<string?>(null);

        public Task SaveSettingAsync(string key, string? plainTextValue, CancellationToken ct = default) =>
            Task.CompletedTask;
    }
}
