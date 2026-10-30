using System.Text.Json.Serialization;

namespace NetAdmin.Providers.UniFi.Models;

// Standard-envelope för UniFi Controller API
internal record UniFiResponse<T>(
    [property: JsonPropertyName("data")] List<T>? Data);

// GET /api/s/{site}/stat/device (Accesspunkter m.m.)
internal record UniFiDeviceDto(
    [property: JsonPropertyName("_id")] string? Id,
    [property: JsonPropertyName("mac")] string? Mac,
    [property: JsonPropertyName("ip")] string? Ip,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("model")] string? Model,
    [property: JsonPropertyName("type")] string? Type,        // "uap" = Access Point
    [property: JsonPropertyName("state")] int State,          // 1 = Online/Connected
    [property: JsonPropertyName("num_sta")] int NumSta);      // Antal anslutna klienter

// GET /api/s/{site}/rest/wlanconf (Konfigurerade SSID)
internal record UniFiWlanConfDto(
    [property: JsonPropertyName("_id")] string? Id,
    [property: JsonPropertyName("name")] string? Ssid,
    [property: JsonPropertyName("enabled")] bool Enabled,
    [property: JsonPropertyName("networkconf_id")] string? NetworkConfId,
    [property: JsonPropertyName("vlan_enabled")] bool VlanEnabled,
    [property: JsonPropertyName("vlan"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] int? LegacyVlanId);

// GET /api/s/{site}/rest/networkconf (Nätverk/VLAN-koppling i UniFi v8/v9)
internal record UniFiNetworkConfDto(
    [property: JsonPropertyName("_id")] string? Id,
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("vlan_enabled")] bool VlanEnabled,
    [property: JsonPropertyName("vlan"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] int? VlanId);

// GET /api/s/{site}/stat/sta (Aktiva klienter)
internal record UniFiClientDto(
    [property: JsonPropertyName("mac")] string? Mac,
    [property: JsonPropertyName("hostname")] string? Hostname,
    [property: JsonPropertyName("name")] string? AliasName,   // Namn satt manuellt i UniFi UI
    [property: JsonPropertyName("is_wired")] bool IsWired,
    [property: JsonPropertyName("ap_mac")] string? ApMac,
    [property: JsonPropertyName("essid")] string? Essid,
    [property: JsonPropertyName("signal")] int? SignalDbm,    // t.ex. -62 (dBm)
    [property: JsonPropertyName("rssi")] int? Rssi,
    [property: JsonPropertyName("radio")] string? Radio,      // "ng" (2.4GHz), "na" (5GHz), "6e" (6GHz)
    [property: JsonPropertyName("channel")] int? Channel);