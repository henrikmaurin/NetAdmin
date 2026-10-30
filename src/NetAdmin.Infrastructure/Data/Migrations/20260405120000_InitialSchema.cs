using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetAdmin.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260405120000_InitialSchema")]
public partial class InitialSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "HardwareDevices",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Category = table.Column<int>(type: "INTEGER", nullable: false),
                IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                MacAddress = table.Column<string>(type: "TEXT", nullable: true),
                Room = table.Column<string>(type: "TEXT", nullable: false),
                PortCount = table.Column<int>(type: "INTEGER", nullable: false),
                ManagementUrl = table.Column<string>(type: "TEXT", nullable: true),
                Notes = table.Column<string>(type: "TEXT", nullable: false),
                Vendor = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                SubModel = table.Column<string>(type: "TEXT", nullable: false),
                ShellyGeneration = table.Column<int>(type: "INTEGER", nullable: false),
                IsManaged = table.Column<bool>(type: "INTEGER", nullable: false),
                SupportedProtocolsCsv = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                MeterInterface = table.Column<string>(type: "TEXT", nullable: false, defaultValue: ""),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HardwareDevices", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "PatchPanelPortItems",
            columns: table => new
            {
                PatchPanelId = table.Column<string>(type: "TEXT", nullable: false),
                PortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                Label = table.Column<string>(type: "TEXT", nullable: false),
                Room = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PatchPanelPortItems", x => new { x.PatchPanelId, x.PortNumber });
            });

        migrationBuilder.CreateTable(
            name: "PatchPanelPorts",
            columns: table => new
            {
                PortNumber = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Label = table.Column<string>(type: "TEXT", nullable: false),
                Room = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PatchPanelPorts", x => x.PortNumber);
            });

        migrationBuilder.CreateTable(
            name: "PhysicalLinks",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                SourceDeviceId = table.Column<string>(type: "TEXT", nullable: false),
                SourceDeviceName = table.Column<string>(type: "TEXT", nullable: false),
                SourcePort = table.Column<int>(type: "INTEGER", nullable: false),
                TargetDeviceId = table.Column<string>(type: "TEXT", nullable: false),
                TargetDeviceName = table.Column<string>(type: "TEXT", nullable: false),
                TargetPort = table.Column<int>(type: "INTEGER", nullable: false),
                CableLabel = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PhysicalLinks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Rooms",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                BuildingOrZone = table.Column<string>(type: "TEXT", nullable: false),
                Floor = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Rooms", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ShellyDevices",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                Model = table.Column<string>(type: "TEXT", nullable: false),
                Room = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShellyDevices", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SwitchPorts",
            columns: table => new
            {
                PortNumber = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Label = table.Column<string>(type: "TEXT", nullable: false),
                Role = table.Column<int>(type: "INTEGER", nullable: false),
                NativeVlanId = table.Column<int>(type: "INTEGER", nullable: false),
                TaggedVlanIdsCsv = table.Column<string>(type: "TEXT", nullable: false),
                PoeEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SwitchPorts", x => x.PortNumber);
            });

        migrationBuilder.CreateTable(
            name: "SystemSettings",
            columns: table => new
            {
                Key = table.Column<string>(type: "TEXT", nullable: false),
                EncryptedValue = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SystemSettings", x => x.Key);
            });

        migrationBuilder.CreateTable(
            name: "UnmanagedSwitches",
            columns: table => new
            {
                Id = table.Column<string>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                PortCount = table.Column<int>(type: "INTEGER", nullable: false),
                Location = table.Column<string>(type: "TEXT", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UnmanagedSwitches", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "HardwareDevices");
        migrationBuilder.DropTable(name: "PatchPanelPortItems");
        migrationBuilder.DropTable(name: "PatchPanelPorts");
        migrationBuilder.DropTable(name: "PhysicalLinks");
        migrationBuilder.DropTable(name: "Rooms");
        migrationBuilder.DropTable(name: "ShellyDevices");
        migrationBuilder.DropTable(name: "SwitchPorts");
        migrationBuilder.DropTable(name: "SystemSettings");
        migrationBuilder.DropTable(name: "UnmanagedSwitches");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
    }
}
