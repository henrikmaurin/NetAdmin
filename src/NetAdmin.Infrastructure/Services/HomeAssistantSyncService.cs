using Microsoft.EntityFrameworkCore;
using NetAdmin.Core.Models;
using NetAdmin.Core.Services;
using NetAdmin.Core.Providers;
using NetAdmin.Infrastructure.Data;

namespace NetAdmin.Infrastructure.Services;

public class HomeAssistantSyncService(
    IDbContextFactory<AppDbContext> dbFactory,
    IHomeAssistantProvider haProvider,
    IHardwareInventoryProvider hardwareProvider) : IHomeAssistantSyncService
{
    public Task<IReadOnlyList<HomeAssistantSyncResult>> SyncEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default) =>
        SyncEntitiesCoreAsync(deviceId, ipOrUrl, serviceId: null, ct);

    public Task<IReadOnlyList<HomeAssistantSyncResult>> SyncServiceEntitiesAsync(string serviceId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            throw new ArgumentException("A Home Assistant service ID is required.", nameof(serviceId));

        return SyncEntitiesCoreAsync(null, null, serviceId, ct);
    }

    private async Task<IReadOnlyList<HomeAssistantSyncResult>> SyncEntitiesCoreAsync(string? deviceId, string? ipOrUrl, string? serviceId, CancellationToken ct)
    {
        var hardware = await hardwareProvider.GetAllHardwareAsync(ct);
        var hardwareById = hardware.ToDictionary(device => device.Id, StringComparer.OrdinalIgnoreCase);
        var instanceDevice = !string.IsNullOrWhiteSpace(deviceId) && hardwareById.TryGetValue(deviceId, out var registeredDevice)
            ? registeredDevice
            : null;

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var existingServices = await db.Services
            .ToDictionaryAsync(service => service.Id, StringComparer.OrdinalIgnoreCase, ct);
        var results = new List<HomeAssistantSyncResult>();
        ServiceEntity? instanceService = null;

        if (serviceId is not null)
        {
            if (!existingServices.TryGetValue(serviceId, out instanceService) ||
                HomeAssistantServiceIds.IsEntityServiceId(instanceService.Id) ||
                !IsHomeAutomationService(instanceService.Type))
                throw new KeyNotFoundException($"Home Assistant service '{serviceId}' was not found.");

            if (instanceDevice is null && instanceService.LinkedDeviceId is { } linkedDeviceId)
                hardwareById.TryGetValue(linkedDeviceId, out instanceDevice);
        }
        else if (instanceDevice is not null)
        {
            var canonicalInstanceId = HomeAssistantServiceIds.GetInstanceServiceId(instanceDevice.Id);
            instanceService = existingServices.GetValueOrDefault(canonicalInstanceId)
                ?? existingServices.Values.FirstOrDefault(service => service.IsServiceInstance && service.LinkedDeviceId == instanceDevice.Id)
                ?? existingServices.Values.FirstOrDefault(service =>
                    service.LinkedDeviceId == instanceDevice.Id &&
                    service.ParentServiceId is null &&
                    !HomeAssistantServiceIds.IsEntityServiceId(service.Id) &&
                    IsHomeAutomationService(service.Type));

            if (instanceService is null)
            {
                instanceService = new ServiceEntity { Id = canonicalInstanceId };
                db.Services.Add(instanceService);
                existingServices.Add(instanceService.Id, instanceService);
            }
        }

        var instanceServiceId = instanceService?.Id;
        if (instanceService is not null)
        {
            if (instanceDevice is not null)
            {
                instanceService.Name = instanceDevice.Name;
                instanceService.LinkedDeviceId = instanceDevice.Id;
            }
            instanceService.Type = ServiceType.HomeAutomation.ToString();
            instanceService.Description = "Home Assistant instance";
            instanceService.ParentServiceId = null;
            instanceService.IsServiceInstance = true;
        }

        var entities = instanceService is not null
            ? await haProvider.GetEntitiesForServiceAsync(instanceService.Id, instanceDevice?.Id, ct)
            : await haProvider.GetEntitiesAsync(deviceId, ipOrUrl, ct);

        foreach (var entity in entities)
        {
            ct.ThrowIfCancellationRequested();
            var candidates = GetNetworkIdentifiers(entity.CandidateDeviceIdentifiers);
            var entityServiceId = HomeAssistantServiceIds.GetEntityServiceId(instanceDevice?.Id ?? instanceServiceId, entity.EntityId);
            existingServices.TryGetValue(entityServiceId, out var service);
            if (service is null && instanceServiceId is not null)
            {
                var legacyServiceId = $"homeassistant:{entity.EntityId}";
                if (existingServices.Remove(legacyServiceId, out var legacyService))
                {
                    db.Services.Remove(legacyService);
                    service = new ServiceEntity
                    {
                        Id = entityServiceId,
                        Name = legacyService.Name,
                        Type = legacyService.Type,
                        Description = legacyService.Description,
                        LinkedDeviceId = legacyService.LinkedDeviceId
                    };
                    db.Services.Add(service);
                    existingServices.Add(entityServiceId, service);
                }
            }

            var linkedDevice = service?.LinkedDeviceId is { } linkedDeviceId && hardwareById.TryGetValue(linkedDeviceId, out var existingDevice)
                ? existingDevice
                : FindMatchingDevice(candidates, hardware);
            var name = entity.FriendlyName ?? entity.EntityId;

            if (service is null)
            {
                service = new ServiceEntity { Id = entityServiceId };
                db.Services.Add(service);
                existingServices.Add(entityServiceId, service);
            }

            service.Name = name;
            service.Type = ServiceType.HomeAutomation.ToString();
            service.Description = entity.AttributesJson;
            service.LinkedDeviceId = linkedDevice?.Id;
            service.ParentServiceId = instanceServiceId;

            results.Add(new HomeAssistantSyncResult(
                service.Id,
                entity.EntityId,
                name,
                linkedDevice?.Id,
                linkedDevice?.Name,
                candidates,
                entity.AttributesJson));
        }

        await db.SaveChangesAsync(ct);
        return results;
    }

    public async Task SetServiceLinkAsync(string serviceId, string? linkedDeviceId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceId) || !HomeAssistantServiceIds.IsEntityServiceId(serviceId))
            throw new ArgumentException("A Home Assistant entity service ID is required.", nameof(serviceId));

        if (!string.IsNullOrWhiteSpace(linkedDeviceId))
        {
            var hardware = await hardwareProvider.GetHardwareByIdAsync(linkedDeviceId, ct);
            if (hardware is null)
                throw new ArgumentException($"Hardware device '{linkedDeviceId}' does not exist.", nameof(linkedDeviceId));
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var service = await db.Services.FirstOrDefaultAsync(item => item.Id == serviceId, ct);
        if (service is null)
            throw new KeyNotFoundException($"Home Assistant entity '{serviceId}' has not been discovered.");

        service.LinkedDeviceId = string.IsNullOrWhiteSpace(linkedDeviceId) ? null : linkedDeviceId;
        await db.SaveChangesAsync(ct);
    }

    private static bool IsHomeAutomationService(string serviceType) =>
        Enum.TryParse<ServiceType>(serviceType, true, out var parsedType) && parsedType == ServiceType.HomeAutomation ||
        string.Equals(serviceType, ServiceType.HomeAutomation.ToDisplayName(), StringComparison.OrdinalIgnoreCase);

    private static IReadOnlyList<string> GetNetworkIdentifiers(IEnumerable<string> candidates) => candidates
        .Where(candidate => TryNormalizeMac(candidate, out _) || TryNormalizeIp(candidate, out _))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private static HardwareDevice? FindMatchingDevice(IReadOnlyList<string> candidates, IReadOnlyList<HardwareDevice> hardware)
    {
        var matchingMacDevices = candidates
            .Select(candidate => TryNormalizeMac(candidate, out var mac) ? mac : null)
            .Where(mac => mac is not null)
            .SelectMany(mac => hardware.Where(device => TryNormalizeMac(device.MacAddress, out var deviceMac) && deviceMac == mac))
            .DistinctBy(device => device.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (matchingMacDevices.Count > 0)
            return matchingMacDevices.Count == 1 ? matchingMacDevices[0] : null;

        var matchingIpDevices = candidates
            .Select(candidate => TryNormalizeIp(candidate, out var ip) ? ip : null)
            .Where(ip => ip is not null)
            .SelectMany(ip => hardware.Where(device => TryNormalizeIp(device.IpAddress, out var deviceIp) && deviceIp == ip))
            .DistinctBy(device => device.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return matchingIpDevices.Count == 1 ? matchingIpDevices[0] : null;
    }

    private static bool TryNormalizeMac(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var hex = value.Where(Uri.IsHexDigit).Select(char.ToUpperInvariant).ToArray();
        if (hex.Length != 12 || value.Any(character => !Uri.IsHexDigit(character) && character is not (':' or '-' or '.')))
            return false;

        normalized = new string(hex);
        return true;
    }

    private static bool TryNormalizeIp(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (!System.Net.IPAddress.TryParse(value, out var address))
            return false;

        normalized = address.ToString();
        return true;
    }
}
