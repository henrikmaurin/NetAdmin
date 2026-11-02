using System.Net.WebSockets;
using System.Text.Json;
using NetAdmin.Core.Models;
using NetAdmin.Core.Providers;
using NetAdmin.Core.Services;

namespace NetAdmin.Providers.HomeAssistant;

public class HomeAssistantProvider(
    HttpClient httpClient,
    IEncryptedSettingsService settingsService)
    : HttpIntegrationProviderBase(httpClient, settingsService), IHomeAssistantProvider, NetAdmin.Core.Providers.IServiceIntegration
{
    public override string ProviderName => "Home Assistant";
    public IReadOnlyCollection<NetAdmin.Core.Models.ServiceType> SupportedServiceTypes => [NetAdmin.Core.Models.ServiceType.HomeAutomation];

    public override async Task<bool> TestConnectionAsync(CancellationToken ct = default)
    {
        var status = await GetStatusAsync(ct: ct);
        return status is not null;
    }

    public Task<HomeAssistantStatusSummary?> GetStatusAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default) =>
        GetStatusCoreAsync(deviceId, ipOrUrl, serviceId: null, ct);

    private async Task<HomeAssistantStatusSummary?> GetStatusCoreAsync(string? deviceId, string? ipOrUrl, string? serviceId, CancellationToken ct)
    {
        var baseUrlRaw = await ResolveServiceSettingAsync(serviceId, deviceId, "api:baseurl", "HomeAssistant:BaseUrl", ipOrUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 8123);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return null;

        var accessToken = await ResolveServiceSettingAsync(serviceId, deviceId, "api:token", "HomeAssistant:AccessToken", null, ct);

        using var configReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/config", bearerToken: accessToken);
        using var configDoc = await SendAndParseJsonAsync(configReq, ct);
        if (configDoc is null)
            return null;

        var root = configDoc.RootElement;
        var version = root.TryGetProperty("version", out var verProp) && verProp.ValueKind == JsonValueKind.String
            ? verProp.GetString() ?? "Unknown"
            : "Unknown";

        var locationName = root.TryGetProperty("location_name", out var locProp) && locProp.ValueKind == JsonValueKind.String
            ? locProp.GetString() ?? "Home"
            : "Home";

        var state = root.TryGetProperty("state", out var stProp) && stProp.ValueKind == JsonValueKind.String
            ? stProp.GetString() ?? "RUNNING"
            : "RUNNING";

        var componentCount = root.TryGetProperty("components", out var compProp) && compProp.ValueKind == JsonValueKind.Array
            ? compProp.GetArrayLength()
            : 0;

        var entityCount = 0;
        using var statesReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/states", bearerToken: accessToken);
        using var statesDoc = await SendAndParseJsonAsync(statesReq, ct);
        if (statesDoc is not null && statesDoc.RootElement.ValueKind == JsonValueKind.Array)
        {
            entityCount = statesDoc.RootElement.GetArrayLength();
        }

        return new HomeAssistantStatusSummary(
            Version: version,
            LocationName: locationName,
            State: state,
            ComponentCount: componentCount,
            EntityCount: entityCount);
    }

    public Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantEntity>> GetEntitiesAsync(string? deviceId = null, string? ipOrUrl = null, CancellationToken ct = default) =>
        GetEntitiesCoreAsync(deviceId, ipOrUrl, serviceId: null, ct);

    public Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantEntity>> GetEntitiesForServiceAsync(string serviceId, string? linkedDeviceId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            throw new ArgumentException("A Home Assistant service ID is required.", nameof(serviceId));

        return GetEntitiesCoreAsync(linkedDeviceId, null, serviceId, ct);
    }

    public async Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantDevice>> GetDevicesForServiceAsync(string serviceId, string? linkedDeviceId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
            throw new ArgumentException("A Home Assistant service ID is required.", nameof(serviceId));

        var baseUrlRaw = await ResolveServiceSettingAsync(serviceId, linkedDeviceId, "api:baseurl", "HomeAssistant:BaseUrl", null, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 8123);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return [];

        var accessToken = await ResolveServiceSettingAsync(serviceId, linkedDeviceId, "api:token", "HomeAssistant:AccessToken", null, ct);
        var registry = await GetHomeAssistantRegistryAsync(baseUrl, accessToken, ct);
        return registry.Devices;
    }

    private async Task<IReadOnlyList<NetAdmin.Core.Models.HomeAssistantEntity>> GetEntitiesCoreAsync(
        string? deviceId,
        string? ipOrUrl,
        string? serviceId,
        CancellationToken ct)
    {
        var baseUrlRaw = await ResolveServiceSettingAsync(serviceId, deviceId, "api:baseurl", "HomeAssistant:BaseUrl", ipOrUrl, ct);
        var baseUrl = NormalizeBaseUrl(baseUrlRaw, "http", 8123);
        if (string.IsNullOrWhiteSpace(baseUrl))
            return Array.Empty<NetAdmin.Core.Models.HomeAssistantEntity>();

        var accessToken = await ResolveServiceSettingAsync(serviceId, deviceId, "api:token", "HomeAssistant:AccessToken", null, ct);

        using var statesReq = CreateAuthenticatedGetRequest($"{baseUrl}/api/states", bearerToken: accessToken);
        using var statesDoc = await SendAndParseJsonAsync(statesReq, ct);
        if (statesDoc is null || statesDoc.RootElement.ValueKind != JsonValueKind.Array)
            return Array.Empty<NetAdmin.Core.Models.HomeAssistantEntity>();

        var registry = await GetHomeAssistantRegistryAsync(baseUrl, accessToken, ct);
        var entities = new List<NetAdmin.Core.Models.HomeAssistantEntity>();
        foreach (var item in statesDoc.RootElement.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.Object ||
                !item.TryGetProperty("entity_id", out var entityIdProperty) || entityIdProperty.ValueKind != JsonValueKind.String)
                continue;

            var entityId = entityIdProperty.GetString() ?? string.Empty;
            var state = item.TryGetProperty("state", out var stateProperty) && stateProperty.ValueKind == JsonValueKind.String
                ? stateProperty.GetString() ?? string.Empty
                : string.Empty;
            var candidates = new List<string>();
            string? friendlyName = null;
            string? deviceClass = null;
            string? unitOfMeasure = null;
            var attributesJson = "{}";

            if (item.TryGetProperty("attributes", out var attributes) && attributes.ValueKind == JsonValueKind.Object)
            {
                attributesJson = attributes.GetRawText();
                if (attributes.TryGetProperty("friendly_name", out var friendlyNameProperty) && friendlyNameProperty.ValueKind == JsonValueKind.String)
                    friendlyName = friendlyNameProperty.GetString();
                if (attributes.TryGetProperty("device_class", out var deviceClassProperty) && deviceClassProperty.ValueKind == JsonValueKind.String)
                    deviceClass = deviceClassProperty.GetString();
                if (attributes.TryGetProperty("unit_of_measurement", out var unitProperty) && unitProperty.ValueKind == JsonValueKind.String)
                    unitOfMeasure = unitProperty.GetString();

                foreach (var attribute in attributes.EnumerateObject())
                {
                    if (!IsNetworkAddressAttribute(attribute.Name))
                        continue;

                    AddCandidateValues(attribute.Value, candidates);
                }
            }

            var isShellyDevice = false;
            if (registry.IdentifiersByEntity.TryGetValue(entityId, out var registeredDevice))
            {
                candidates.AddRange(registeredDevice.Identifiers);
                isShellyDevice = registeredDevice.IsShellyDevice;
            }

            entities.Add(new NetAdmin.Core.Models.HomeAssistantEntity(
                EntityId: entityId,
                State: state,
                FriendlyName: friendlyName,
                DeviceClass: deviceClass,
                UnitOfMeasure: unitOfMeasure,
                AttributesJson: attributesJson,
                CandidateDeviceIdentifiers: candidates.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                IsShellyDevice: isShellyDevice));
        }

        return entities;
    }

    private sealed record RegisteredDeviceIdentifiers(IReadOnlyList<string> Identifiers, bool IsShellyDevice);

    private sealed record HomeAssistantRegistry(
        IReadOnlyList<NetAdmin.Core.Models.HomeAssistantDevice> Devices,
        IReadOnlyDictionary<string, RegisteredDeviceIdentifiers> IdentifiersByEntity);

    private sealed record RegisteredHomeAssistantDevice(
        string Id,
        string Name,
        string? Manufacturer,
        string? Model,
        IReadOnlyList<string> Identifiers,
        bool IsShellyDevice);

    private static async Task<HomeAssistantRegistry> GetHomeAssistantRegistryAsync(
        string baseUrl,
        string? accessToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new InvalidOperationException("A Home Assistant access token is required to read its device registry.");

        var baseUri = new Uri(baseUrl, UriKind.Absolute);
        var websocketUri = new UriBuilder(baseUri)
        {
            Scheme = baseUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? "wss" : "ws",
            Path = $"{baseUri.AbsolutePath.TrimEnd('/')}/api/websocket",
            Query = string.Empty,
            Fragment = string.Empty
        }.Uri;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(15));
        using var socket = new ClientWebSocket();
        try
        {
            await socket.ConnectAsync(websocketUri, timeoutCts.Token);
            using (var authRequired = await ReceiveWebSocketJsonAsync(socket, timeoutCts.Token))
            {
                if (!authRequired.RootElement.TryGetProperty("type", out var authType) || authType.GetString() != "auth_required")
                    throw new InvalidOperationException("Home Assistant did not request WebSocket authentication.");
            }

            await SendWebSocketJsonAsync(socket, new { type = "auth", access_token = accessToken }, timeoutCts.Token);
            using (var authResult = await ReceiveWebSocketJsonAsync(socket, timeoutCts.Token))
            {
                if (!authResult.RootElement.TryGetProperty("type", out var authType) || authType.GetString() != "auth_ok")
                    throw new InvalidOperationException("Home Assistant rejected WebSocket authentication.");
            }

            await SendWebSocketJsonAsync(socket, new { id = 1, type = "config/device_registry/list" }, timeoutCts.Token);
            using var devicesResponse = await ReceiveWebSocketResultAsync(socket, 1, timeoutCts.Token);
            await SendWebSocketJsonAsync(socket, new { id = 2, type = "config/entity_registry/list" }, timeoutCts.Token);
            using var entitiesResponse = await ReceiveWebSocketResultAsync(socket, 2, timeoutCts.Token);

            var registeredDevices = new Dictionary<string, RegisteredHomeAssistantDevice>(StringComparer.OrdinalIgnoreCase);
            if (devicesResponse.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var device in devicesResponse.RootElement.EnumerateArray())
                {
                    if (!device.TryGetProperty("id", out var idProperty) || idProperty.ValueKind != JsonValueKind.String)
                        continue;

                    var identifiers = new List<string>();
                    AddRegistryIdentifiers(device, "connections", identifiers);
                    AddRegistryIdentifiers(device, "identifiers", identifiers);
                    var id = idProperty.GetString()!;
                    var manufacturer = GetStringProperty(device, "manufacturer");
                    var model = GetStringProperty(device, "model");
                    var name = GetStringProperty(device, "name_by_user") ?? GetStringProperty(device, "name") ?? manufacturer ?? model ?? id;
                    registeredDevices[id] = new RegisteredHomeAssistantDevice(
                        id,
                        name,
                        manufacturer,
                        model,
                        identifiers.Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                        IsShellyRegistryDevice(device));
                }
            }

            var entityIdsByDevice = registeredDevices.Keys.ToDictionary(id => id, _ => new List<string>(), StringComparer.OrdinalIgnoreCase);
            var identifiersByEntity = new Dictionary<string, RegisteredDeviceIdentifiers>(StringComparer.OrdinalIgnoreCase);
            if (entitiesResponse.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var entity in entitiesResponse.RootElement.EnumerateArray())
                {
                    if (!entity.TryGetProperty("entity_id", out var entityIdProperty) || entityIdProperty.ValueKind != JsonValueKind.String ||
                        !entity.TryGetProperty("device_id", out var deviceIdProperty) || deviceIdProperty.ValueKind != JsonValueKind.String ||
                        !registeredDevices.TryGetValue(deviceIdProperty.GetString()!, out var device))
                        continue;

                    var entityId = entityIdProperty.GetString()!;
                    entityIdsByDevice[device.Id].Add(entityId);
                    identifiersByEntity[entityId] = new RegisteredDeviceIdentifiers(device.Identifiers, device.IsShellyDevice);
                }
            }

            var devices = registeredDevices.Values
                .Select(device => new NetAdmin.Core.Models.HomeAssistantDevice(
                    device.Id,
                    device.Name,
                    device.Manufacturer,
                    device.Model,
                    device.Identifiers,
                    entityIdsByDevice[device.Id],
                    device.IsShellyDevice))
                .OrderBy(device => device.Name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            return new HomeAssistantRegistry(devices, identifiersByEntity);
        }
        catch (OperationCanceledException ex) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException("Timed out while reading the Home Assistant device registry.", ex);
        }
    }

    private static string? GetStringProperty(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static bool IsShellyRegistryDevice(JsonElement device)
    {
        if (device.TryGetProperty("manufacturer", out var manufacturer) && manufacturer.ValueKind == JsonValueKind.String &&
            manufacturer.GetString()?.Contains("Shelly", StringComparison.OrdinalIgnoreCase) == true)
            return true;

        if (!device.TryGetProperty("identifiers", out var identifiers) || identifiers.ValueKind != JsonValueKind.Array)
            return false;

        return identifiers.EnumerateArray().Any(pair =>
            pair.ValueKind == JsonValueKind.Array && pair.GetArrayLength() > 0 && pair[0].ValueKind == JsonValueKind.String &&
            pair[0].GetString()?.Equals("shelly", StringComparison.OrdinalIgnoreCase) == true);
    }

    private static void AddRegistryIdentifiers(JsonElement device, string propertyName, ICollection<string> identifiers)
    {
        if (!device.TryGetProperty(propertyName, out var values) || values.ValueKind != JsonValueKind.Array)
            return;

        foreach (var pair in values.EnumerateArray())
        {
            if (pair.ValueKind == JsonValueKind.Array && pair.GetArrayLength() > 1 && pair[1].ValueKind == JsonValueKind.String)
            {
                var identifier = pair[1].GetString();
                if (!string.IsNullOrWhiteSpace(identifier))
                    identifiers.Add(identifier.Trim());
            }
        }
    }

    private static async Task SendWebSocketJsonAsync(ClientWebSocket socket, object message, CancellationToken ct)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(message);
        await socket.SendAsync(new ArraySegment<byte>(payload), WebSocketMessageType.Text, true, ct);
    }

    private static async Task<JsonDocument> ReceiveWebSocketResultAsync(ClientWebSocket socket, int expectedId, CancellationToken ct)
    {
        while (true)
        {
            var response = await ReceiveWebSocketJsonAsync(socket, ct);
            var root = response.RootElement;
            if (!root.TryGetProperty("id", out var idProperty) || !idProperty.TryGetInt32(out var id) || id != expectedId)
            {
                response.Dispose();
                continue;
            }

            if (root.TryGetProperty("success", out var successProperty) && successProperty.ValueKind == JsonValueKind.False)
            {
                var error = root.TryGetProperty("error", out var errorProperty) ? errorProperty.GetRawText() : "Unknown registry error.";
                response.Dispose();
                throw new InvalidOperationException($"Home Assistant rejected a registry request: {error}");
            }

            if (!root.TryGetProperty("result", out var result))
            {
                response.Dispose();
                throw new InvalidOperationException("Home Assistant returned an invalid registry response.");
            }

            return JsonDocument.Parse(result.GetRawText());
        }
    }

    private static async Task<JsonDocument> ReceiveWebSocketJsonAsync(ClientWebSocket socket, CancellationToken ct)
    {
        using var message = new MemoryStream();
        var buffer = new byte[8192];
        WebSocketReceiveResult result;
        do
        {
            result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            if (result.MessageType == WebSocketMessageType.Close)
                throw new WebSocketException("Home Assistant closed the WebSocket connection during registry discovery.");
            if (result.MessageType != WebSocketMessageType.Text)
                throw new WebSocketException("Home Assistant returned a non-text WebSocket message.");

            message.Write(buffer, 0, result.Count);
        } while (!result.EndOfMessage);

        return JsonDocument.Parse(message.ToArray());
    }

    private static bool IsNetworkAddressAttribute(string name)
    {
        var normalizedName = name.Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();

        return normalizedName is "ip" or "ipaddress" or "ipv4" or "ipv4address" or "ipv6" or "ipv6address"
            or "mac" or "macaddress" or "macaddr" or "hardwareaddress";
    }

    private static void AddCandidateValues(JsonElement value, ICollection<string> candidates)
    {
        if (value.ValueKind == JsonValueKind.String)
        {
            var candidate = value.GetString();
            if (!string.IsNullOrWhiteSpace(candidate))
                candidates.Add(candidate.Trim());
            return;
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in value.EnumerateArray())
                AddCandidateValues(item, candidates);
        }
    }

    public async Task<NetAdmin.Core.Models.ServiceStatus?> GetServiceStatusAsync(string? serviceId = null, string? baseUrl = null, string? linkedDeviceId = null, CancellationToken ct = default)
    {
        var deviceId = HomeAssistantServiceIds.GetInstanceDeviceId(serviceId) ?? linkedDeviceId ?? serviceId;
        var sum = await GetStatusCoreAsync(deviceId, baseUrl, serviceId, ct);
        if (sum is null) return null;

        var summary = $"Location={sum.LocationName}; Entities={sum.EntityCount}; Components={sum.ComponentCount}";
        return new NetAdmin.Core.Models.ServiceStatus(ProviderName, sum.State, sum.Version, summary, sum);
    }

    private async Task<string?> ResolveServiceSettingAsync(
        string? serviceId,
        string? deviceId,
        string deviceKeySuffix,
        string globalSettingKey,
        string? fallbackValue,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(serviceId))
        {
            var serviceValue = await SettingsService.GetSettingAsync($"service:{serviceId}:{deviceKeySuffix}", null, ct);
            if (!string.IsNullOrWhiteSpace(serviceValue))
                return serviceValue.Trim();
        }

        return await ResolveSettingAsync(deviceId, deviceKeySuffix, globalSettingKey, fallbackValue, ct);
    }
}
