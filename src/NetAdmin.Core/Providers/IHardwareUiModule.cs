using NetAdmin.Core.Models;

namespace NetAdmin.Core.Providers;

/// <summary>
/// Kontrakt för en självständig hårdvarumodul som kapslar in både domänlogik (instansiering/konvertering)
/// och sina egna Blazor-komponenter (formulär, specialsektioner, kortdetaljer och knappar).
/// </summary>
public interface IHardwareUiModule
{
    string ModuleId { get; }
    string DisplayName { get; }
    string Icon { get; }
    HardwareCategory Category { get; }

    /// <summary>
    /// Högre värde prioriteras före mer generella moduler inom samma kategori (t.ex. Shelly > generisk IoT).
    /// </summary>
    int Priority => 0;

    /// <summary>
    /// Avgör om modulen ansvarar för en befintlig hårdvaruinstans.
    /// </summary>
    bool CanHandle(HardwareDevice device);

    /// <summary>
    /// Avgör om modulen matchar en viss kombination av kategori, tillverkare och taggar.
    /// </summary>
    bool Matches(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null);

    /// <summary>
    /// Skapar en ny standardinstans för denna hårdvarutyp.
    /// </summary>
    HardwareDevice CreateDefault(string id, string name = "", string? ipAddress = null, string? macAddress = null, string room = "");

    /// <summary>
    /// Säkerställer att en HardwareDevice är konverterad till rätt konkret subtyp för denna modul
    /// med bibehållna gemensamma fält.
    /// </summary>
    HardwareDevice EnsureDeviceType(HardwareDevice current);

    /// <summary>
    /// Blazor-komponent för redigering av typens egna fält och specialsektioner (tar Device och DeviceChanged).
    /// </summary>
    Type? EditorComponentType => null;

    /// <summary>
    /// Blazor-komponent för extra åtgärdsknappar i detaljvy/dashboard (tar Device).
    /// </summary>
    Type? HeaderActionsComponentType => null;

    /// <summary>
    /// Blazor-komponent för typens sammanfattning på kortet i hårdvarukatalogen (tar Device).
    /// </summary>
    Type? CardSummaryComponentType => null;

    /// <summary>
    /// Returns the declared capabilities for a device handled by this module.
    /// Default: no capabilities. Providers/modules may override to declare supported actions.
    /// </summary>
    IEnumerable<NetAdmin.Core.Models.DeviceCapability> GetCapabilities(HardwareDevice device) => System.Linq.Enumerable.Empty<NetAdmin.Core.Models.DeviceCapability>();

    /// <summary>
    /// Returns the API and credential fields relevant for a specific device (e.g. API keys, SSH credentials, tokens).
    /// Return empty list for passive devices that do not use credentials.
    /// </summary>
    IReadOnlyList<DeviceCredentialField> GetCredentialFields(HardwareDevice device) => [];
}

public class HardwareTypeRegistry(IEnumerable<IHardwareUiModule> modules)
{
    private readonly IReadOnlyList<IHardwareUiModule> _modules = modules
        .OrderByDescending(m => m.Priority)
        .ThenBy(m => m.DisplayName)
        .ToList();

    public IReadOnlyList<IHardwareUiModule> AllModules => _modules;

    public IHardwareUiModule Resolve(HardwareDevice device)
    {
        return _modules.FirstOrDefault(m => m.CanHandle(device))
            ?? _modules.FirstOrDefault(m => m.Category == device.Category)
            ?? _modules.Last();
    }

    public IHardwareUiModule ResolveByCriteria(HardwareCategory category, string? vendor = null, string? tagsCsv = null, string? hostname = null)
    {
        return _modules.FirstOrDefault(m => m.Matches(category, vendor, tagsCsv, hostname))
            ?? _modules.FirstOrDefault(m => m.Category == category)
            ?? _modules.Last();
    }

    public IHardwareUiModule? GetByModuleId(string moduleId)
    {
        return _modules.FirstOrDefault(m => m.ModuleId.Equals(moduleId, StringComparison.OrdinalIgnoreCase));
    }

    public string GetCategoryIcon(HardwareCategory category)
    {
        return _modules.LastOrDefault(m => m.Category == category)?.Icon ?? "💻";
    }
}
