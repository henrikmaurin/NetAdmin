using System.Net.Sockets;

namespace NetAdmin.Providers.Asus;

public class AsusProbeResult
{
    public bool HasWeb { get; set; }
    public bool HasSsh { get; set; }
    public string? ServerHeader { get; set; }
    public string? HtmlSnippet { get; set; }
}

public class AsusService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AsusService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AsusProbeResult> ProbeAsync(string ipOrHost)
    {
        var result = new AsusProbeResult();

        // Check HTTP(S)
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(4);

            // Try HTTPS then HTTP
            HttpResponseMessage? resp = null;
            try { resp = await client.GetAsync($"https://{ipOrHost}/"); }
            catch { }
            if (resp is null || !resp.IsSuccessStatusCode)
            {
                try { resp = await client.GetAsync($"http://{ipOrHost}/"); }
                catch { }
            }

            if (resp != null)
            {
                result.HasWeb = true;
                if (resp.Headers.Server != null)
                    result.ServerHeader = resp.Headers.Server.ToString();
                try { result.HtmlSnippet = await resp.Content.ReadAsStringAsync(); }
                catch { }
            }
        }
        catch { /* swallow */ }

        // Check SSH port 22
        try
        {
            using var tcp = new TcpClient();
            var task = tcp.ConnectAsync(ipOrHost, 22);
            var ok = await Task.WhenAny(task, Task.Delay(1200)) == task && tcp.Connected;
            result.HasSsh = ok;
        }
        catch { result.HasSsh = false; }

        return result;
    }

    public async Task<string?> ExecuteSshCommandAsync(string host, int port, string username, string password, string command)
    {
        try
        {
            return await AsusSshClient.ExecuteCommandAsync(host, port, username, password, command);
        }
        catch
        {
            return null;
        }
    }
}
