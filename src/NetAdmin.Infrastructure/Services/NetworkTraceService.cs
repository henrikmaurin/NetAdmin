using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;

namespace NetAdmin.Infrastructure.Services;

public record TraceHop(
    int StepNumber,
    string DeviceId,
    string DeviceName,
    string Category,
    int? InPort,
    int? OutPort,
    string CableLabel,
    string Room,
    string IpAddress);

public class NetworkTraceService(
    IHardwareInventoryProvider hardwareProvider,
    IPhysicalLinkProvider linkProvider,
    IEnumerable<IWirelessProvider> wirelessProviders)
{
    public async Task<IReadOnlyList<TraceHop>> TracePathAsync(
        string startDeviceId,
        string? targetDeviceId = null,
        CancellationToken ct = default)
    {
        var allHardware = await hardwareProvider.GetAllHardwareAsync(ct);
        var allLinks = await linkProvider.GetLinksAsync(ct);

        var hardwareDict = allHardware.ToDictionary(h => h.Id, h => h, StringComparer.OrdinalIgnoreCase);

        var hops = new List<TraceHop>();
        if (!hardwareDict.TryGetValue(startDeviceId, out var startDevice))
        {
            return hops;
        }

        int step = 1;
        hops.Add(new TraceHop(
            StepNumber: step,
            DeviceId: startDevice.Id,
            DeviceName: startDevice.Name,
            Category: startDevice.Category.ToDisplayName(),
            InPort: null,
            OutPort: null,
            CableLabel: "Start Device",
            Room: startDevice.Room,
            IpAddress: startDevice.IpAddress ?? ""));

        string currentNodeId = startDevice.Id;

        // 1. Kolla om startenheten saknar egen fysisk kabel och är trådlöst ansluten via en AP
        bool hasDirectCable = allLinks.Any(l =>
            l.SourceDeviceId.Equals(currentNodeId, StringComparison.OrdinalIgnoreCase) ||
            l.TargetDeviceId.Equals(currentNodeId, StringComparison.OrdinalIgnoreCase));

        if (!hasDirectCable && !string.IsNullOrWhiteSpace(startDevice.MacAddress))
        {
            var wirelessHop = await TryResolveWirelessApHopAsync(startDevice, allHardware, allLinks, step + 1, ct);
            if (wirelessHop is not null)
            {
                step++;
                hops.Add(wirelessHop);
                // Sätt currentNodeId till den registrerade AP:ns riktiga ID i HardwareInventory!
                currentNodeId = wirelessHop.DeviceId;
            }
        }

        // 2. Bestäm slutdestination: angiven enhet eller Internet (WanEndpoint)
        var goalDevice = (!string.IsNullOrWhiteSpace(targetDeviceId) && hardwareDict.TryGetValue(targetDeviceId, out var customTarget))
            ? customTarget
            : allHardware.FirstOrDefault(h => h.Category == HardwareCategory.WanEndpoint)
              ?? allHardware.FirstOrDefault(h => h.Category == HardwareCategory.Router);

        if (goalDevice is null || currentNodeId.Equals(goalDevice.Id, StringComparison.OrdinalIgnoreCase))
        {
            return hops;
        }

        // 3. Hitta den fysiska kabelvägen (BFS) från currentNodeId (t.ex. AP:n) hela vägen till målet
        var cablePath = FindShortestCablePath(currentNodeId, goalDevice.Id, allLinks);

        if (cablePath.Count > 0)
        {
            string pointerId = currentNodeId;
            foreach (var link in cablePath)
            {
                bool isSource = link.SourceDeviceId.Equals(pointerId, StringComparison.OrdinalIgnoreCase);
                string nextId = isSource ? link.TargetDeviceId : link.SourceDeviceId;

                if (!hardwareDict.TryGetValue(nextId, out var nextDev)) break;

                step++;
                hops.Add(new TraceHop(
                    StepNumber: step,
                    DeviceId: nextDev.Id,
                    DeviceName: nextDev.Name,
                    Category: nextDev.Category.ToDisplayName(),
                    InPort: isSource ? link.SourcePort : link.TargetPort,
                    OutPort: isSource ? link.TargetPort : link.SourcePort,
                    CableLabel: string.IsNullOrWhiteSpace(link.CableLabel) ? "Network Cable" : link.CableLabel,
                    Room: nextDev.Room,
                    IpAddress: nextDev.IpAddress ?? ""));

                pointerId = nextId;
            }
        }

        return hops;
    }

    private async Task<TraceHop?> TryResolveWirelessApHopAsync(
        HardwareDevice startDevice,
        IReadOnlyList<HardwareDevice> allHardware,
        IReadOnlyList<PhysicalLink> allLinks,
        int stepNumber,
        CancellationToken ct)
    {
        var normClientMac = MacAddress.Normalize(startDevice.MacAddress!);

        foreach (var wp in wirelessProviders)
        {
            try
            {
                var wirelessClients = await wp.GetWirelessClientsAsync(ct);
                var accessPoints = await wp.GetAccessPointsAsync(ct);

                var client = wirelessClients.FirstOrDefault(c =>
                    MacAddress.Normalize(c.MacAddress).Equals(normClientMac, StringComparison.OrdinalIgnoreCase));

                if (client is null) continue;

                var apMacOrName = GetPropertyString(client, "AccessPointMac", "ApMac", "ConnectedApMac", "Bssid")
                               ?? GetPropertyString(client, "AccessPointName", "ApName", "AccessPoint");

                var matchedUniFiAp = accessPoints.FirstOrDefault(a =>
                    !string.IsNullOrWhiteSpace(apMacOrName) &&
                    (MacAddress.Normalize(a.MacAddress).Equals(MacAddress.Normalize(apMacOrName), StringComparison.OrdinalIgnoreCase) ||
                     a.Name.Equals(apMacOrName, StringComparison.OrdinalIgnoreCase)))
                    ?? accessPoints.FirstOrDefault();

                // Hitta den registrerade AP-enheten i vår egen HardwareInventory!
                // Vi matchar på: 1) MAC-adress, 2) IP-adress, 3) Namn, eller 4) en registrerad enhet som har fysisk kabel och heter "AP"/"UniFi" eller har kategorin AccessPoint
                var apHardware =
                    allHardware.FirstOrDefault(h =>
                        matchedUniFiAp is not null &&
                        !string.IsNullOrWhiteSpace(h.MacAddress) &&
                        MacAddress.Normalize(h.MacAddress).Equals(MacAddress.Normalize(matchedUniFiAp.MacAddress), StringComparison.OrdinalIgnoreCase))
                    ?? allHardware.FirstOrDefault(h =>
                        matchedUniFiAp is not null &&
                        !string.IsNullOrWhiteSpace(h.IpAddress) &&
                        h.IpAddress.Equals(matchedUniFiAp.IpAddress, StringComparison.OrdinalIgnoreCase))
                    ?? allHardware.FirstOrDefault(h =>
                        matchedUniFiAp is not null &&
                        (h.Name.Contains(matchedUniFiAp.Name, StringComparison.OrdinalIgnoreCase) ||
                         matchedUniFiAp.Name.Contains(h.Name, StringComparison.OrdinalIgnoreCase)))
                    ?? allHardware.FirstOrDefault(h =>
                        h.Category == HardwareCategory.AccessPoint &&
                        allLinks.Any(l => l.SourceDeviceId.Equals(h.Id, StringComparison.OrdinalIgnoreCase) ||
                                          l.TargetDeviceId.Equals(h.Id, StringComparison.OrdinalIgnoreCase)))
                    ?? allHardware.FirstOrDefault(h => h.Category == HardwareCategory.AccessPoint);

                if (apHardware is not null)
                {
                    return new TraceHop(
                        StepNumber: stepNumber,
                        DeviceId: apHardware.Id,
                        DeviceName: apHardware.Name,
                        Category: apHardware.Category.ToDisplayName(),
                        InPort: null,
                        OutPort: 1,
                        CableLabel: $"📶 Wi-Fi ({client.Ssid}, {client.SignalRssi} dBm)",
                        Room: apHardware.Room,
                        IpAddress: apHardware.IpAddress ?? matchedUniFiAp?.IpAddress ?? "");
                }
            }
            catch
            {
                // Ignorera vid API-fel
            }
        }

        return null;
    }

    private static List<PhysicalLink> FindShortestCablePath(
        string startId,
        string targetId,
        IReadOnlyList<PhysicalLink> allLinks)
    {
        var queue = new Queue<(string CurrentNode, List<PhysicalLink> Path)>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { startId };

        queue.Enqueue((startId, []));

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            if (current.Equals(targetId, StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            var connectedLinks = allLinks.Where(l =>
                l.SourceDeviceId.Equals(current, StringComparison.OrdinalIgnoreCase) ||
                l.TargetDeviceId.Equals(current, StringComparison.OrdinalIgnoreCase));

            foreach (var link in connectedLinks)
            {
                string neighbor = link.SourceDeviceId.Equals(current, StringComparison.OrdinalIgnoreCase)
                    ? link.TargetDeviceId
                    : link.SourceDeviceId;

                if (visited.Add(neighbor))
                {
                    var newPath = new List<PhysicalLink>(path) { link };
                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        return [];
    }

    private static string? GetPropertyString(object obj, params string[] candidateNames)
    {
        var type = obj.GetType();
        foreach (var name in candidateNames)
        {
            var prop = type.GetProperty(name);
            if (prop?.GetValue(obj) is string val && !string.IsNullOrWhiteSpace(val))
            {
                return val;
            }
        }
        return null;
    }
}