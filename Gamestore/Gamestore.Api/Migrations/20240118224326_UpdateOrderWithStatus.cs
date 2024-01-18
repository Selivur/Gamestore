using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gamestore.Api.Migrations;

/// <inheritdoc />
public partial class UpdateOrderWithStatus : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "PaymentDate",
            table: "Orders",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "Orders",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PaymentDate",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "Orders");
    }
}
