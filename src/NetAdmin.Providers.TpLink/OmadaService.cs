namespace NetAdmin.Providers.TpLink;

public class OmadaProbeResult
{
    public bool HasWeb { get; set; }
    public bool HasApi { get; set; }
}

public class OmadaService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OmadaService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<OmadaProbeResult> ProbeAsync(string ipOrHost)
    {
        var r = new OmadaProbeResult();
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
            if (resp != null) r.HasWeb = true;
        }
        catch { }

        return r;
    }
}
