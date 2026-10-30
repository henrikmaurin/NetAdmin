using System.Text.Json.Serialization;

namespace NetAdmin.Providers.OpnSense.Models;

// Standard-envelope för OPNsense search-endpoints
internal record OpnSenseSearchResponse<T>(
    [property: JsonPropertyName("rows")] List<T>? Rows,
    [property: JsonPropertyName("total")] int Total);

// GET /api/interfaces/vlan_settings/searchItem
internal record OpnSenseVlanRow(
    [property: JsonPropertyName("uuid")] string? Uuid,
    [property: JsonPropertyName("if")] string? ParentInterface,
    [property: JsonPropertyName("tag"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] int VlanTag,
    [property: JsonPropertyName("descr")] string? Description,
    [property: JsonPropertyName("vlanif")] string? VlanInterfaceName);

// GET /api/diagnostics/interface/getArp
internal record OpnSenseArpDto(
    [property: JsonPropertyName("mac")] string? Mac,
    [property: JsonPropertyName("ip")] string? Ip,
    [property: JsonPropertyName("intf")] string? InterfaceId,
    [property: JsonPropertyName("intf_description")] string? InterfaceDescription,
    [property: JsonPropertyName("hostname")] string? Hostname,
    [property: JsonPropertyName("permanent")] bool Permanent,
    [property: JsonPropertyName("expired")] bool Expired);

// GET /api/kea/leases4/search (Kea DHCPv4)
internal record OpnSenseKeaLeaseRow(
    [property: JsonPropertyName("address")] string? IpAddress,
    [property: JsonPropertyName("hwaddr")] string? MacAddress,
    [property: JsonPropertyName("hostname")] string? Hostname,
    [property: JsonPropertyName("expire"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] long? ExpireUnix,
    [property: JsonPropertyName("if_descr")] string? InterfaceDescription);

// GET /api/dhcpv4/leases/searchLease (Legacy ISC DHCPv4)
internal record OpnSenseIscLeaseRow(
    [property: JsonPropertyName("address")] string? IpAddress,
    [property: JsonPropertyName("mac")] string? MacAddress,
    [property: JsonPropertyName("hostname")] string? Hostname,
    [property: JsonPropertyName("type")] string? LeaseType, // "static" eller "dynamic"
    [property: JsonPropertyName("descr")] string? Description);