using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetAdmin.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260405130000_AddDeviceIdToSwitchPorts")]
public partial class AddDeviceIdToSwitchPorts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SwitchPorts_New",
            columns: table => new
            {
                DeviceId = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "tplink-sg108pe-1"),
                PortNumber = table.Column<int>(type: "INTEGER", nullable: false),
                Label = table.Column<string>(type: "TEXT", nullable: false),
                Role = table.Column<int>(type: "INTEGER", nullable: false),
                NativeVlanId = table.Column<int>(type: "INTEGER", nullable: false),
                TaggedVlanIdsCsv = table.Column<string>(type: "TEXT", nullable: false),
                PoeEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                TagsCsv = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SwitchPorts", x => new { x.DeviceId, x.PortNumber });
            });

        migrationBuilder.Sql("""
            INSERT INTO "SwitchPorts_New" ("DeviceId", "PortNumber", "Label", "Role", "NativeVlanId", "TaggedVlanIdsCsv", "PoeEnabled", "TagsCsv")
            SELECT 'tplink-sg108pe-1', "PortNumber", "Label", "Role", "NativeVlanId", "TaggedVlanIdsCsv", "PoeEnabled", "TagsCsv"
            FROM "SwitchPorts";
            """);

        migrationBuilder.DropTable(name: "SwitchPorts");

        migrationBuilder.RenameTable(
            name: "SwitchPorts_New",
            newName: "SwitchPorts");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SwitchPorts_Old",
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

        migrationBuilder.Sql("""
            INSERT INTO "SwitchPorts_Old" ("PortNumber", "Label", "Role", "NativeVlanId", "TaggedVlanIdsCsv", "PoeEnabled", "TagsCsv")
            SELECT "PortNumber", "Label", "Role", "NativeVlanId", "TaggedVlanIdsCsv", "PoeEnabled", "TagsCsv"
            FROM "SwitchPorts"
            WHERE "DeviceId" = 'tplink-sg108pe-1';
            """);

        migrationBuilder.DropTable(name: "SwitchPorts");

        migrationBuilder.RenameTable(
            name: "SwitchPorts_Old",
            newName: "SwitchPorts");
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13");
    }
}
