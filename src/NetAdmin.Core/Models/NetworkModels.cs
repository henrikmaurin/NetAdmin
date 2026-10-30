namespace NetAdmin.Core.Models;

public static class MacAddress
{
    public static string Normalize(string? rawMac) =>
        string.IsNullOrWhiteSpace(rawMac)
            ? string.Empty
            : rawMac.Trim().Replace('-', ':').ToUpperInvariant();
}

// --- Layer 3 / Routing / Brandvägg / DHCP ---
public record NetworkInterface(
    string Identifier,      // t.ex. "igc0", "vlan01"
    string Name,            // t.ex. "LAN", "IoT_VLAN"
    string? IpAddress,
    int? SubnetPrefix,
    int? VlanTag,
    bool IsUp);

public record VlanDefinition(
    int VlanId,             // t.ex. 10, 20, 30
    string Name,            // t.ex. "IoT", "Guest", "Servers"
    string ParentInterface, // t.ex. "igc1"
    string? SubnetCidr);    // t.ex. "192.168.20.0/24"

public record ArpEntry(
    string MacAddress,
    string IpAddress,
    string InterfaceName,
    bool IsPermanent);

public record DhcpLease(
    string MacAddress,
    string IpAddress,
    string? Hostname,
    bool IsStatic,
    DateTimeOffset? ExpiresAt);

public record FirewallRule(
    string Id,
    string InterfaceName,
    string Action,          // Pass, Block, Reject
    string Protocol,
    string Source,
    string Destination,
    string Description,
    bool IsEnabled);

// --- WiFi / Accesspunkter ---
public record AccessPoint(
    string Id,
    string Name,
    string MacAddress,
    string IpAddress,
    string Model,
    bool IsOnline,
    int ConnectedClientsCount);

public record WirelessNetwork(
    string Id,
    string Ssid,
    int? VlanId,            // Null eller 1 för otaggat standardnät
    bool IsEnabled);

public record WirelessClient(
    string MacAddress,
    string? Hostname,
    string ApMacAddress,
    string ApName,
    string Ssid,
    int? SignalRssi,        // t.ex. -58 dBm
    string? RadioBand);     // t.ex. "5 GHz", "2.4 GHz"