namespace NetAdmin.Core.Models;

public record CorrelatedClient(
    string MacAddress,
    string? IpAddress,
    string? Hostname,
    string? InterfaceOrVlan,
    bool IsStaticDhcp,
    bool IsOnline,
    bool IsWireless,
    string? ConnectedApName,
    string? Ssid,
    int? SignalRssi,
    int? EstimatedSwitchPort);