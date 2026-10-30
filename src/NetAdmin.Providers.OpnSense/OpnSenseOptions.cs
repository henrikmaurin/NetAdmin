namespace NetAdmin.Providers.OpnSense;

public class OpnSenseOptions
{
    public const string SectionName = "OpnSense";

    public string BaseUrl { get; set; } = "https://192.168.1.1";
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public bool IgnoreSslErrors { get; set; } = true;
}