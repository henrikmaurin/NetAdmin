using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetAdmin.Infrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261101160000_AddHomeAssistantServiceOwnership")]
public partial class AddHomeAssistantServiceOwnership : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ParentServiceId",
            table: "Services",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsServiceInstance",
            table: "Services",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "IX_Services_ParentServiceId",
            table: "Services",
            column: "ParentServiceId");

        migrationBuilder.CreateIndex(
            name: "IX_Services_LinkedDeviceId_IsServiceInstance",
            table: "Services",
            columns: new[] { "LinkedDeviceId", "IsServiceInstance" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Services_LinkedDeviceId_IsServiceInstance",
            table: "Services");

        migrationBuilder.DropIndex(
            name: "IX_Services_ParentServiceId",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "IsServiceInstance",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "ParentServiceId",
            table: "Services");
    }
}
