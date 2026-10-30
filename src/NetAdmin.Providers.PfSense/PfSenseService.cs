namespace NetAdmin.Providers.PfSense;

public class PfSenseProbeResult
{
    public bool HasWeb { get; set; }
    public bool HasApi { get; set; }
    public string? ServerHeader { get; set; }
}

public class PfSenseService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PfSenseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PfSenseProbeResult> ProbeAsync(string ipOrHost)
    {
        var r = new PfSenseProbeResult();
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(4);
            HttpResponseMessage? resp = null;
            try { resp = await client.GetAsync($"https://{ipOrHost}/"); } catch { }
            if (resp is null || !resp.IsSuccessStatusCode)
            {
                try { resp = await client.GetAsync($"http://{ipOrHost}/"); } catch { }
            }
            if (resp != null)
            {
                r.HasWeb = true;
                if (resp.Headers.Server != null) r.ServerHeader = resp.Headers.Server.ToString();
            }
        }
        catch { }

        return r;
    }
}
