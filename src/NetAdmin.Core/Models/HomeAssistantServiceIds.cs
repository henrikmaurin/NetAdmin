namespace NetAdmin.Core.Models;

public static class HomeAssistantServiceIds
{
    private const string InstancePrefix = "homeassistant:instance:";
    private const string EntityPrefix = "homeassistant:entity:";
    private const string LegacyPrefix = "homeassistant:";

    public static string GetInstanceServiceId(string deviceId) =>
        $"{InstancePrefix}{Uri.EscapeDataString(deviceId)}";

    public static string GetEntityServiceId(string? deviceId, string entityId) =>
        string.IsNullOrWhiteSpace(deviceId)
            ? $"{LegacyPrefix}{entityId}"
            : $"{EntityPrefix}{Uri.EscapeDataString(deviceId)}:{Uri.EscapeDataString(entityId)}";

    public static string? GetInstanceDeviceId(string? serviceId) =>
        serviceId is not null && serviceId.StartsWith(InstancePrefix, StringComparison.OrdinalIgnoreCase)
            ? Uri.UnescapeDataString(serviceId[InstancePrefix.Length..])
            : null;

    public static string GetEntityIdentifier(string serviceId)
    {
        if (serviceId.StartsWith(EntityPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var separatorIndex = serviceId.IndexOf(':', EntityPrefix.Length);
            if (separatorIndex >= 0)
                return Uri.UnescapeDataString(serviceId[(separatorIndex + 1)..]);
        }

        return serviceId.StartsWith(LegacyPrefix, StringComparison.OrdinalIgnoreCase)
            ? serviceId[LegacyPrefix.Length..]
            : serviceId;
    }

    public static bool IsInstanceServiceId(string serviceId) =>
        serviceId.StartsWith(InstancePrefix, StringComparison.OrdinalIgnoreCase);

    public static bool IsEntityServiceId(string serviceId) =>
        serviceId.StartsWith(LegacyPrefix, StringComparison.OrdinalIgnoreCase) && !IsInstanceServiceId(serviceId);
}
