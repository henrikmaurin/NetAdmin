using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Providers.TpLink;

public class TpLinkSg108PeProvider(
    ISwitchPortStore portStore,
    IHardwareInventoryProvider hardwareProvider,
    IPhysicalLinkProvider linkProvider) : ISwitchProvider
{
    public string ProviderName => "Switch & Network Port Configuration Engine";

    public Task<bool> TestConnectionAsync(CancellationToken ct = default) =>
        Task.FromResult(true);

    public async Task<IReadOnlyList<HardwareDevice>> GetConfigurableDevicesAsync(CancellationToken ct = default)
    {
        var all = await hardwareProvider.GetAllHardwareAsync(ct);

        return all
            .Where(d => d.Category is HardwareCategory.Switch or HardwareCategory.Router ||
                        (d.PortCount > 1 && d.Category is not HardwareCategory.PatchPanel))
            .OrderBy(d => d.Category switch
            {
                HardwareCategory.Switch => 0,
                HardwareCategory.Router => 1,
                HardwareCategory.AccessPoint => 2,
                HardwareCategory.FiberConverter => 3,
                _ => 4
            })
            .ThenBy(d => d.Name)
            .ToList();
    }

    public async Task<SwitchDevice> GetSwitchAsync(CancellationToken ct = default)
    {
        var devices = await GetConfigurableDevicesAsync(ct);
        var defaultDevice = devices.FirstOrDefault(d => d.Id == "tplink-sg108pe-1")
                            ?? devices.FirstOrDefault(d => d is SwitchHardwareDevice { IsManaged: true })
                            ?? devices.FirstOrDefault();

        return await GetSwitchAsync(defaultDevice?.Id ?? "tplink-sg108pe-1", ct);
    }

    public async Task<SwitchDevice> GetSwitchAsync(string deviceId, CancellationToken ct = default)
    {
        var targetId = string.IsNullOrWhiteSpace(deviceId) ? "tplink-sg108pe-1" : deviceId;
        var hw = await hardwareProvider.GetHardwareByIdAsync(targetId, ct);

        if (hw is null)
        {
            var devices = await GetConfigurableDevicesAsync(ct);
            hw = devices.FirstOrDefault();
            if (hw is not null)
            {
                targetId = hw.Id;
            }
        }

        var portCount = hw is not null ? Math.Max(1, hw.PortCount) : 8;
        var storedPorts = await portStore.GetPortsAsync(targetId, portCount, ct);
        var allLinks = await linkProvider.GetLinksAsync(ct);

        var deviceLinks = allLinks
            .Where(l => l.SourceDeviceId.Equals(targetId, StringComparison.OrdinalIgnoreCase) ||
                        l.TargetDeviceId.Equals(targetId, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var allHw = await hardwareProvider.GetAllHardwareAsync(ct);
        var hwById = allHw.ToDictionary(d => d.Id, StringComparer.OrdinalIgnoreCase);

        var enrichedPorts = new List<SwitchPortDefinition>(storedPorts.Count);
        foreach (var port in storedPorts)
        {
            var link = deviceLinks.FirstOrDefault(l =>
                (l.SourceDeviceId.Equals(targetId, StringComparison.OrdinalIgnoreCase) && l.SourcePort == port.PortNumber) ||
                (l.TargetDeviceId.Equals(targetId, StringComparison.OrdinalIgnoreCase) && l.TargetPort == port.PortNumber));

            string? connectedId = null;
            string? connectedName = null;
            int? remotePort = null;
            string? cableLabel = null;
            var label = port.Label;
            var role = port.Role;

            if (link is not null)
            {
                var isSource = link.SourceDeviceId.Equals(targetId, StringComparison.OrdinalIgnoreCase);
                connectedId = isSource ? link.TargetDeviceId : link.SourceDeviceId;
                connectedName = isSource ? link.TargetDeviceName : link.SourceDeviceName;
                remotePort = isSource ? link.TargetPort : link.SourcePort;
                cableLabel = link.CableLabel;

                // If port still has generic default label, enrich from physical cabling
                if (string.IsNullOrWhiteSpace(label) || label.Equals($"Port {port.PortNumber}", StringComparison.OrdinalIgnoreCase))
                {
                    label = !string.IsNullOrWhiteSpace(connectedName)
                        ? $"{connectedName} (Port {remotePort})"
                        : label;
                }

                // Suggest role if port is still Unused and connected to a known Router or Access Point
                if (role == SwitchPortRole.Unused && connectedId is not null && hwById.TryGetValue(connectedId, out var remoteHw))
                {
                    if (remoteHw.Category == HardwareCategory.Router)
                        role = SwitchPortRole.RouterTrunk;
                    else if (remoteHw.Category == HardwareCategory.AccessPoint)
                        role = SwitchPortRole.AccessPointTrunk;
                    else
                        role = SwitchPortRole.AccessPort;
                }
            }

            enrichedPorts.Add(port with
            {
                DeviceId = targetId,
                Label = label,
                Role = role,
                ConnectedDeviceId = connectedId,
                ConnectedDeviceName = connectedName,
                ConnectedRemotePort = remotePort,
                CableLabel = cableLabel
            });
        }

        if (hw is null)
        {
            return new SwitchDevice(
                Id: targetId,
                Name: "TP-Link TL-SG108PE",
                Model: "TL-SG108PE (8-Port Gigabit PoE)",
                IsManagedViaApi: false,
                Ports: enrichedPorts,
                Vendor: "TP-Link",
                Category: HardwareCategory.Switch,
                Room: "Server Closet",
                IpAddress: "192.168.1.2",
                ManagementUrl: "http://192.168.1.2",
                IsVlanCapable: true,
                SupportsTpLinkEasySmartGuide: true);
        }

        var vendor = hw.Vendor;
        var model = hw is SwitchHardwareDevice swDev && !string.IsNullOrWhiteSpace(swDev.SwitchModel)
            ? swDev.SwitchModel
            : hw.Model;
        var isVlanCapable = hw switch
        {
            SwitchHardwareDevice sw => sw.IsManaged,
            _ => hw.Category is HardwareCategory.Router or HardwareCategory.AccessPoint ||
                 hw.TagsCsv.Contains("vlan", StringComparison.OrdinalIgnoreCase)
        };

        var isTpLinkEasySmart = hw is SwitchHardwareDevice { IsManaged: true } tpSw &&
            (tpSw.Vendor.Contains("TP-Link", StringComparison.OrdinalIgnoreCase) ||
             tpSw.SwitchModel.Contains("SG10", StringComparison.OrdinalIgnoreCase) ||
             tpSw.TagsCsv.Contains("tplink", StringComparison.OrdinalIgnoreCase) ||
             tpSw.Id.Contains("tplink", StringComparison.OrdinalIgnoreCase));

        return new SwitchDevice(
            Id: hw.Id,
            Name: hw.Name,
            Model: string.IsNullOrWhiteSpace(model) ? hw.Category.ToDisplayName() : model,
            IsManagedViaApi: false,
            Ports: enrichedPorts,
            Vendor: vendor,
            Category: hw.Category,
            Room: hw.Room,
            IpAddress: hw.IpAddress,
            ManagementUrl: hw.EffectiveManagementUrl,
            IsVlanCapable: isVlanCapable,
            SupportsTpLinkEasySmartGuide: isTpLinkEasySmart);
    }

    public Task UpdatePortAsync(SwitchPortDefinition port, CancellationToken ct = default) =>
        portStore.SavePortAsync(port, ct);

    public async Task<SwitchConfigurationPlan> GeneratePlaybookAsync(
        IReadOnlyList<VlanDefinition> routerVlans,
        IReadOnlyList<WirelessNetwork> wifiNetworks,
        CancellationToken ct = default)
    {
        var sw = await GetSwitchAsync(ct);
        return await GeneratePlaybookAsync(sw.Id, routerVlans, wifiNetworks, ct);
    }

    public async Task<SwitchConfigurationPlan> GeneratePlaybookAsync(
        string deviceId,
        IReadOnlyList<VlanDefinition> routerVlans,
        IReadOnlyList<WirelessNetwork> wifiNetworks,
        CancellationToken ct = default)
    {
        var sw = await GetSwitchAsync(deviceId, ct);
        var ports = sw.Ports;
        var warnings = new List<string>();

        // 1. Collect all known VLANs (Default VLAN 1 + Router VLANs + Wi-Fi VLANs + Port VLANs)
        var vlanNames = new Dictionary<int, string>
        {
            [1] = "Default / Management LAN"
        };

        foreach (var rv in routerVlans)
        {
            vlanNames[rv.VlanId] = rv.Name;
        }

        var wifiVlanIds = wifiNetworks
            .Where(w => w.IsEnabled && w.VlanId is > 1)
            .Select(w => w.VlanId!.Value)
            .Distinct()
            .ToList();

        foreach (var wifi in wifiNetworks.Where(w => w.IsEnabled && w.VlanId is > 1))
        {
            var vid = wifi.VlanId!.Value;
            if (!vlanNames.ContainsKey(vid))
            {
                vlanNames[vid] = $"Wi-Fi: {wifi.Ssid}";
                warnings.Add($"SSID '{wifi.Ssid}' uses VLAN {vid}, but VLAN {vid} was not found on the router.");
            }
        }

        foreach (var p in ports)
        {
            if (!vlanNames.ContainsKey(p.NativeVlanId))
                vlanNames[p.NativeVlanId] = $"Port {p.PortNumber} Native";

            foreach (var t in p.TaggedVlanIds.Where(t => !vlanNames.ContainsKey(t)))
                vlanNames[t] = $"Tagged VLAN {t}";
        }

        // 2. Topology validation
        if (sw.Category == HardwareCategory.Switch && sw.IsVlanCapable && !ports.Any(p => p.Role == SwitchPortRole.RouterTrunk))
        {
            warnings.Add($"No port on '{sw.Name}' is configured as 'Router Trunk / Uplink'. Tagged VLAN traffic will not reach the router.");
        }

        if (sw.Model.Contains("SG108PE", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var p in ports.Where(p => p.PoeEnabled && p.PortNumber > 4))
            {
                warnings.Add($"Port {p.PortNumber} ({p.Label}) has PoE enabled, but on TL-SG108PE only ports 1–4 support PoE.");
            }
        }

        // 3. Calculate 802.1Q VLAN rows (Untagged / Tagged / Not Member per VLAN)
        var vlanRows = new List<TpLinkVlanConfigRow>();

        foreach (var (vlanId, vlanName) in vlanNames.OrderBy(kv => kv.Key))
        {
            var untagged = new List<int>();
            var tagged = new List<int>();
            var notMember = new List<int>();

            foreach (var port in ports.OrderBy(p => p.PortNumber))
            {
                if (port.TaggedVlanIds.Contains(vlanId))
                {
                    tagged.Add(port.PortNumber);
                    continue;
                }

                switch (port.Role)
                {
                    case SwitchPortRole.RouterTrunk:
                        if (vlanId == port.NativeVlanId)
                            untagged.Add(port.PortNumber);
                        else
                            tagged.Add(port.PortNumber);
                        break;

                    case SwitchPortRole.AccessPointTrunk:
                        if (vlanId == port.NativeVlanId)
                            untagged.Add(port.PortNumber);
                        else if (wifiVlanIds.Contains(vlanId))
                            tagged.Add(port.PortNumber);
                        else
                            notMember.Add(port.PortNumber);
                        break;

                    case SwitchPortRole.CustomTrunk:
                    case SwitchPortRole.AccessPort:
                        if (vlanId == port.NativeVlanId)
                            untagged.Add(port.PortNumber);
                        else
                            notMember.Add(port.PortNumber);
                        break;

                    case SwitchPortRole.Unused:
                    default:
                        if (vlanId == 1)
                            untagged.Add(port.PortNumber);
                        else
                            notMember.Add(port.PortNumber);
                        break;
                }
            }

            vlanRows.Add(new TpLinkVlanConfigRow(
                VlanId: vlanId,
                VlanName: vlanName,
                UntaggedPorts: untagged,
                TaggedPorts: tagged,
                NotMemberPorts: notMember));
        }

        // 4. Calculate 802.1Q PVID Setting per port
        var pvidRows = ports
            .OrderBy(p => p.PortNumber)
            .Select(p => new TpLinkPvidConfigRow(
                PortNumber: p.PortNumber,
                PortLabel: p.Label,
                Pvid: p.Role == SwitchPortRole.Unused ? 1 : p.NativeVlanId))
            .ToList();

        return new SwitchConfigurationPlan(
            SwitchName: sw.Name,
            VlanTable: vlanRows,
            PvidTable: pvidRows,
            ValidationWarnings: warnings);
    }
}