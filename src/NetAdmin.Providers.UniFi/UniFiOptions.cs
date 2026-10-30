namespace NetAdmin.Providers.UniFi;

public class UniFiOptions
{
    public const string SectionName = "UniFi";

    public string BaseUrl { get; set; } = "https://192.168.1.10:8443";
    public string Site { get; set; } = "default";

    // Alternativ 1: API-nyckel (UniFi Network 8.5+)
    public string? ApiKey { get; set; }

    // Alternativ 2: Lokal Read-Only-användare (Fristående Docker-kontroller / äldre UniFi)
    public string? Username { get; set; }
    public string? Password { get; set; }

    // Sätt till true om du kör UniFi OS (UDM / CloudKey Gen2+ på port 443)
    public bool IsUniFiOs { get; set; } = false;
    public bool IgnoreSslErrors { get; set; } = true;
}