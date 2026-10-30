using NetAdmin.Core.Models;

namespace NetAdmin.Providers.Epson.Products;

public sealed class EpsonEt2650PrinterProduct : HardwareProductDefinition
{
    public override string ProductId => "epson-et-2650";
    public override string Vendor => "Epson";
    public override string Model => "EcoTank ET-2650";
    public override HardwareCategory Category => HardwareCategory.Printer;
    public override string SubCategory => "Printers & Scanners";
    public override int DefaultPortCount => 0;
    public override IReadOnlyList<string> DeviceCodes => ["ET-2650", "ET2650", "EPSON", "EcoTank"];
    public override string DefaultTagsCsv => "printer, scanner, epson, ecotank, wifi";
    public override string Description => "Epson EcoTank ET-2650 Wireless All-in-One Supertank Printer / Scanner (Wi-Fi / Wi-Fi Direct)";
}

public sealed class EpsonEcoTankNetworkPrinterProduct : HardwareProductDefinition
{
    public override string ProductId => "epson-ecotank-workforce-pro";
    public override string Vendor => "Epson";
    public override string Model => "EcoTank / WorkForce Pro Network Printer";
    public override HardwareCategory Category => HardwareCategory.Printer;
    public override string SubCategory => "Printers & Scanners";
    public override int DefaultPortCount => 1;
    public override IReadOnlyList<string> DeviceCodes => ["ET-2850", "ET-3850", "ET-4850", "WorkForce"];
    public override string DefaultTagsCsv => "printer, scanner, epson, ecotank, lan, wifi";
    public override string Description => "Epson EcoTank / WorkForce Pro All-in-One Printer with Wi-Fi & Ethernet";
}
