using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetAdmin.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261031124500_AddNvrFkIndex")]
    public partial class AddNvrFkIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Recreate HardwareDevices table to add FK constraint on NvrId referencing HardwareDevices(Id)
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=off;
                BEGIN TRANSACTION;
                CREATE TABLE ""HardwareDevices_new"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""Category"" INTEGER NOT NULL,
                    ""IpAddress"" TEXT,
                    ""IsManaged"" INTEGER NOT NULL,
                    ""MacAddress"" TEXT,
                    ""ManagementUrl"" TEXT,
                    ""MeterInterface"" TEXT NOT NULL,
                    ""Name"" TEXT NOT NULL,
                    ""Notes"" TEXT NOT NULL,
                    ""PortCount"" INTEGER NOT NULL,
                    ""Room"" TEXT NOT NULL,
                    ""ShellyGeneration"" INTEGER NOT NULL,
                    ""StreamsJson"" TEXT NOT NULL,
                    ""SubModel"" TEXT NOT NULL,
                    ""SupportedProtocolsCsv"" TEXT NOT NULL,
                    ""TagsCsv"" TEXT NOT NULL,
                    ""Vendor"" TEXT NOT NULL,
                    ""NvrId"" TEXT,
                    FOREIGN KEY(""NvrId"") REFERENCES ""HardwareDevices""(""Id"") ON DELETE SET NULL
                );

                INSERT INTO ""HardwareDevices_new"" (""Id"",""Category"",""IpAddress"",""IsManaged"",""MacAddress"",""ManagementUrl"",""MeterInterface"",""Name"",""Notes"",""PortCount"",""Room"",""ShellyGeneration"",""StreamsJson"",""SubModel"",""SupportedProtocolsCsv"",""TagsCsv"",""Vendor"",""NvrId"")
                SELECT ""Id"",""Category"",""IpAddress"",""IsManaged"",""MacAddress"",""ManagementUrl"",""MeterInterface"",""Name"",""Notes"",""PortCount"",""Room"",""ShellyGeneration"",""StreamsJson"",""SubModel"",""SupportedProtocolsCsv"",""TagsCsv"",""Vendor"",""NvrId"" FROM ""HardwareDevices"";

                DROP TABLE ""HardwareDevices"";
                ALTER TABLE ""HardwareDevices_new"" RENAME TO ""HardwareDevices"";
                CREATE INDEX ""IX_HardwareDevices_NvrId"" ON ""HardwareDevices"" (""NvrId"");
                COMMIT;
                PRAGMA foreign_keys=on;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                PRAGMA foreign_keys=off;
                BEGIN TRANSACTION;
                CREATE TABLE ""HardwareDevices_old"" (
                    ""Id"" TEXT NOT NULL PRIMARY KEY,
                    ""Category"" INTEGER NOT NULL,
                    ""IpAddress"" TEXT,
                    ""IsManaged"" INTEGER NOT NULL,
                    ""MacAddress"" TEXT,
                    ""ManagementUrl"" TEXT,
                    ""MeterInterface"" TEXT NOT NULL,
                    ""Name"" TEXT NOT NULL,
                    ""Notes"" TEXT NOT NULL,
                    ""PortCount"" INTEGER NOT NULL,
                    ""Room"" TEXT NOT NULL,
                    ""ShellyGeneration"" INTEGER NOT NULL,
                    ""StreamsJson"" TEXT NOT NULL,
                    ""SubModel"" TEXT NOT NULL,
                    ""SupportedProtocolsCsv"" TEXT NOT NULL,
                    ""TagsCsv"" TEXT NOT NULL,
                    ""Vendor"" TEXT NOT NULL
                );

                INSERT INTO ""HardwareDevices_old"" (""Id"",""Category"",""IpAddress"",""IsManaged"",""MacAddress"",""ManagementUrl"",""MeterInterface"",""Name"",""Notes"",""PortCount"",""Room"",""ShellyGeneration"",""StreamsJson"",""SubModel"",""SupportedProtocolsCsv"",""TagsCsv"",""Vendor"")
                SELECT ""Id"",""Category"",""IpAddress"",""IsManaged"",""MacAddress"",""ManagementUrl"",""MeterInterface"",""Name"",""Notes"",""PortCount"",""Room"",""ShellyGeneration"",""StreamsJson"",""SubModel"",""SupportedProtocolsCsv"",""TagsCsv"",""Vendor"" FROM ""HardwareDevices"";

                DROP INDEX IF EXISTS ""IX_HardwareDevices_NvrId"";
                DROP TABLE ""HardwareDevices"";
                ALTER TABLE ""HardwareDevices_old"" RENAME TO ""HardwareDevices"";
                COMMIT;
                PRAGMA foreign_keys=on;
            ");
        }
    }
}
