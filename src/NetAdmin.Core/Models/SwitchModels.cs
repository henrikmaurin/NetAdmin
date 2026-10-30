namespace NetAdmin.Core.Models;

public enum SwitchPortRole
{
    Unused = 0,
    RouterTrunk = 1,        // Uplink / Trunk to Router or Core Switch (Tagged for all or selected VLANs)
    AccessPointTrunk = 2,   // To Wi-Fi Access Point (Untagged Mgmt VLAN, Tagged SSID VLANs)
    AccessPort = 3,         // Client / Server / Endpoint (Untagged for 1 specific VLAN + PVID)
    CustomTrunk = 4         // Custom Trunk / Hybrid Port (Untagged Native VLAN + specific Tagged VLANs)
}

public record SwitchPortDefinition(
    int PortNumber,
    string Label,
    SwitchPortRole Role,
    int NativeVlanId,
    IReadOnlyList<int> TaggedVlanIds,
    bool PoeEnabled,
    string DeviceId = "tplink-sg108pe-1",
    string? ConnectedDeviceId = null,
    string? ConnectedDeviceName = null,
    int? ConnectedRemotePort = null,
    string? CableLabel = null);

public record SwitchDevice(
    string Id,
    string Name,
    string Model,
    bool IsManagedViaApi,
    IReadOnlyList<SwitchPortDefinition> Ports,
    string Vendor = "",
    HardwareCategory Category = HardwareCategory.Switch,
    string Room = "",
    string? IpAddress = null,
    string? ManagementUrl = null,
    bool IsVlanCapable = true,
    bool SupportsTpLinkEasySmartGuide = false);

// Represents a row in TP-Link menu "VLAN -> 802.1Q VLAN Configuration"
public record TpLinkVlanConfigRow(
    int VlanId,
    string VlanName,
    IReadOnlyList<int> UntaggedPorts,
    IReadOnlyList<int> TaggedPorts,
    IReadOnlyList<int> NotMemberPorts)
{
    public IReadOnlyList<int> MemberPorts =>
        UntaggedPorts.Concat(TaggedPorts).Distinct().OrderBy(x => x).ToList();
}

// Represents a row in 802.1Q PVID configuration table
public record TpLinkPvidConfigRow(
    int PortNumber,
    string PortLabel,
    int Pvid);

public record SwitchConfigurationPlan(
    string SwitchName,
    IReadOnlyList<TpLinkVlanConfigRow> VlanTable,
    IReadOnlyList<TpLinkPvidConfigRow> PvidTable,
    IReadOnlyList<string> ValidationWarnings);