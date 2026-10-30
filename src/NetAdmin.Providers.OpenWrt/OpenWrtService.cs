namespace NetAdmin.Providers.OpenWrt;

public class OpenWrtProbeResult
{
    public bool HasWeb { get; set; }
    public bool HasUbus { get; set; }
}

public class OpenWrtService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OpenWrtService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<OpenWrtProbeResult> ProbeAsync(string ipOrHost)
    {
        var r = new OpenWrtProbeResult();
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(4);
            HttpResponseMessage? resp = null;
            try { resp = await client.GetAsync($"http://{ipOrHost}/"); } catch { }
            if (resp != null)
            {
                r.HasWeb = true;
            }
        }
        catch { }

        return r;
    }
}
