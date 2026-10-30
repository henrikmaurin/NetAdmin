using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Infrastructure.Services;

public class NetworkTopologyService(
    IEnumerable<IRoutingProvider> routingProviders,
    IEnumerable<IDhcpProvider> dhcpProviders,
    IEnumerable<IWirelessProvider> wirelessProviders)
{
    public async Task<IReadOnlyList<CorrelatedClient>> GetCorrelatedClientsAsync(CancellationToken ct = default)
    {
        // Hämta från alla kapabiliteter parallellt
        var arpTasks = routingProviders.Select(p => p.GetArpTableAsync(ct));
        var dhcpTasks = dhcpProviders.Select(p => p.GetLeasesAsync(ct));
        var wifiTasks = wirelessProviders.Select(p => p.GetWirelessClientsAsync(ct));

        var arpEntries = (await Task.WhenAll(arpTasks)).SelectMany(x => x).ToList();
        var dhcpLeases = (await Task.WhenAll(dhcpTasks)).SelectMany(x => x).ToList();
        var wifiClients = (await Task.WhenAll(wifiTasks)).SelectMany(x => x).ToList();

        // Indexera på normaliserad MAC-adress
        var arpByMac = arpEntries
            .Where(a => !string.IsNullOrEmpty(a.MacAddress))
            .GroupBy(a => MacAddress.Normalize(a.MacAddress))
            .ToDictionary(g => g.Key, g => g.First());

        var dhcpByMac = dhcpLeases
            .Where(d => !string.IsNullOrEmpty(d.MacAddress))
            .GroupBy(d => MacAddress.Normalize(d.MacAddress))
            .ToDictionary(g => g.Key, g => g.First());

        var wifiByMac = wifiClients
            .Where(w => !string.IsNullOrEmpty(w.MacAddress))
            .GroupBy(w => MacAddress.Normalize(w.MacAddress))
            .ToDictionary(g => g.Key, g => g.First());

        // Slå ihop alla unika MAC-adresser som syns i nätverket
        var allMacs = arpByMac.Keys
            .Union(dhcpByMac.Keys)
            .Union(wifiByMac.Keys)
            .OrderBy(m => m);

        var result = new List<CorrelatedClient>();

        foreach (var mac in allMacs)
        {
            arpByMac.TryGetValue(mac, out var arp);
            dhcpByMac.TryGetValue(mac, out var dhcp);
            wifiByMac.TryGetValue(mac, out var wifi);

            result.Add(new CorrelatedClient(
                MacAddress: mac,
                IpAddress: arp?.IpAddress ?? dhcp?.IpAddress,
                Hostname: dhcp?.Hostname ?? wifi?.Hostname,
                InterfaceOrVlan: arp?.InterfaceName,
                IsStaticDhcp: dhcp?.IsStatic ?? false,
                IsOnline: arp is not null || wifi is not null,
                IsWireless: wifi is not null,
                ConnectedApName: wifi?.ApName,
                Ssid: wifi?.Ssid,
                SignalRssi: wifi?.SignalRssi,
                EstimatedSwitchPort: null // Kopplas på i Etapp 4 via Switch Twin
            ));
        }

        return result;
    }
}