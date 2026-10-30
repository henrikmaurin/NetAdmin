namespace NetAdmin.Core.Models;

/// <summary>
/// Basklass för en starkt typad produktdefinition (en C#-klass per produkt).
/// Varje provider (Shelly, TP-Link, UniFi, OPNsense, etc.) kan definiera sina egna
/// produktklasser som ärver från denna eller en provider-specifik basklass.
/// </summary>
public abstract class HardwareProductDefinition
{
    /// <summary>Unikt produkt-ID i katalogen (t.ex. "shelly-plus-1pm", "tplink-tl-sg108pe").</summary>
    public abstract string ProductId { get; }

    /// <summary>Tillverkare (t.ex. "Shelly", "TP-Link", "Ubiquiti UniFi").</summary>
    public abstract string Vendor { get; }

    /// <summary>Modellnamn (t.ex. "Plus 1PM", "Pro 4PM", "TL-SG108PE").</summary>
    public abstract string Model { get; }

    /// <summary>Huvudkategori i infrastrukturen.</summary>
    public abstract HardwareCategory Category { get; }

    /// <summary>Produktfamilj / underkategori (t.ex. "Smart Switches & Relays", "Professional DIN", "Energy Meters").</summary>
    public abstract string SubCategory { get; }

    /// <summary>Interna enhetskoder / modellkoder som enheten själv rapporterar (t.ex. "SNSW-001P16EU", "Plus1PM").</summary>
    public virtual IReadOnlyList<string> DeviceCodes => [];

    /// <summary>Generation (t.ex. 1, 2, 3, 4 för Shelly).</summary>
    public virtual int Generation => 0;

    /// <summary>Antal fysiska RJ45/nätverksportar som produkten har som standard (0 för ren Wi-Fi, 1+ för LAN/Pro).</summary>
    public virtual int DefaultPortCount => 0;

    /// <summary>Standardtaggar för produkten.</summary>
    public virtual string DefaultTagsCsv => "";

    /// <summary>Beskrivning eller teknisk specifikation (t.ex. "1-kanals 16A relä med effektmätning, Wi-Fi + BLE").</summary>
    public virtual string Description => "";

    /// <summary>Länk till produktsida.</summary>
    public virtual string ProductUrl => "";

    public string FullName => $"{Vendor} {Model}".Trim();

    /// <summary>
    /// Kontrollerar om denna produktklass matchar en modellsträng, intern enhetskod eller app-kod.
    /// </summary>
    public virtual bool MatchesCodeOrModel(string? codeOrModel)
    {
        if (string.IsNullOrWhiteSpace(codeOrModel))
            return false;

        var trimmed = codeOrModel.Trim();
        if (Model.Equals(trimmed, StringComparison.OrdinalIgnoreCase) ||
            FullName.Equals(trimmed, StringComparison.OrdinalIgnoreCase) ||
            ProductId.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return DeviceCodes.Any(code => code.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Applicerar produktens egenskaper på en HardwareDevice-instans.
    /// </summary>
    public virtual HardwareDevice ApplyToDevice(HardwareDevice device) =>
        device with
        {
            Category = Category,
            Vendor = Vendor,
            Model = Model,
            PortCount = DefaultPortCount,
            TagsCsv = string.IsNullOrWhiteSpace(device.TagsCsv) ? DefaultTagsCsv : device.TagsCsv
        };
}

/// <summary>
/// Abstrakt basklass för IoT-hubbar / Smart Hem-gateways där man kan deklarera vilka protokoll/tekniker som stöds.
/// </summary>
public abstract class IoTHubProductDefinition : HardwareProductDefinition
{
    public override HardwareCategory Category => HardwareCategory.IoTHub;
    public override string SubCategory => "Smart Home Hubs & Bridges";

    /// <summary>Protokoll och tekniker som hubben stöder (t.ex. "Zigbee, Matter, Thread, Ethernet").</summary>
    public abstract string SupportedProtocolsCsv { get; }

    public override HardwareDevice ApplyToDevice(HardwareDevice device)
    {
        if (device is IoTHubHardwareDevice hub)
        {
            return hub with
            {
                Category = HardwareCategory.IoTHub,
                Vendor = Vendor,
                HubModel = Model,
                Model = Model,
                PortCount = DefaultPortCount,
                SupportedProtocolsCsv = SupportedProtocolsCsv,
                TagsCsv = DefaultTagsCsv
            };
        }

        return new IoTHubHardwareDevice(
            Id: device.Id,
            Name: device.Name,
            IpAddress: device.IpAddress,
            MacAddress: device.MacAddress,
            Room: device.Room,
            Vendor: Vendor,
            HubModel: Model,
            PortCount: DefaultPortCount,
            SupportedProtocolsCsv: SupportedProtocolsCsv,
            ManagementUrl: device.ManagementUrl,
            Notes: device.Notes,
            TagsCsv: DefaultTagsCsv);
    }
}

/// <summary>
/// DI-registrerat register som samlar alla registrerade produktklasser (HardwareProductDefinition)
/// från samtliga laddade provider-projekt.
/// </summary>
public class ProductCatalogRegistry(IEnumerable<HardwareProductDefinition> products)
{
    private readonly IReadOnlyList<HardwareProductDefinition> _products = products
        .OrderBy(p => p.Vendor)
        .ThenBy(p => p.SubCategory)
        .ThenBy(p => p.Model)
        .ToList();

    public IReadOnlyList<HardwareProductDefinition> AllProducts => _products;

    public IReadOnlyList<HardwareProductDefinition> GetByVendor(string vendor) =>
        _products.Where(p => p.Vendor.Equals(vendor, StringComparison.OrdinalIgnoreCase)).ToList();

    public IReadOnlyList<HardwareProductDefinition> GetByCategory(HardwareCategory category) =>
        _products.Where(p => p.Category == category).ToList();

    public IReadOnlyList<string> GetVendorsByCategory(HardwareCategory category) =>
        _products
            .Where(p => p.Category == category)
            .Select(p => p.Vendor)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v.Equals("Generic", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
            .ThenBy(v => v)
            .ToList();

    public IReadOnlyList<HardwareProductDefinition> GetByCategoryAndVendor(HardwareCategory category, string vendor) =>
        _products
            .Where(p => p.Category == category && p.Vendor.Equals(vendor, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Model)
            .ToList();

    public IReadOnlyList<TProduct> GetProductsOfType<TProduct>() where TProduct : HardwareProductDefinition =>
        _products.OfType<TProduct>().ToList();

    public HardwareProductDefinition? GetById(string productId) =>
        _products.FirstOrDefault(p => p.ProductId.Equals(productId, StringComparison.OrdinalIgnoreCase));

    public HardwareProductDefinition? FindMatch(string? vendor, string? codeOrModel)
    {
        if (string.IsNullOrWhiteSpace(codeOrModel))
            return null;

        var candidates = string.IsNullOrWhiteSpace(vendor)
            ? _products
            : _products.Where(p => p.Vendor.Equals(vendor, StringComparison.OrdinalIgnoreCase));

        return candidates.FirstOrDefault(p => p.MatchesCodeOrModel(codeOrModel));
    }
}

/// <summary>
/// Resultat från live-identifiering mot en Shelly-enhet på det lokala nätverket (/shelly eller /rpc/Shelly.GetDeviceInfo).
/// </summary>
public record ShellyProbedInfo(
    string IpAddress,
    string? DeviceName,
    string? MacAddress,
    string RawModelCode,
    string? AppCode,
    int Generation,
    string? FirmwareVersion,
    HardwareProductDefinition? MatchedProduct)
{
    public string FriendlyModel =>
        MatchedProduct?.Model
        ?? (!string.IsNullOrWhiteSpace(AppCode) ? AppCode : RawModelCode);

    public int EffectivePortCount =>
        MatchedProduct?.DefaultPortCount
        ?? (FriendlyModel.Contains("Pro", StringComparison.OrdinalIgnoreCase) ? 1 : 0);

    public string EffectiveTagsCsv =>
        MatchedProduct?.DefaultTagsCsv
        ?? $"shelly, iot, gen{Generation}";
}

