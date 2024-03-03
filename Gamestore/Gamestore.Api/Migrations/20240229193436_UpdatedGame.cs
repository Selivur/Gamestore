using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gamestore.Api.Migrations;

/// <inheritdoc />
public partial class UpdatedGame : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders");

        migrationBuilder.AlterColumn<int>(
            name: "CustomerId",
            table: "Orders",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<DateTime>(
            name: "PublishDate",
            table: "Games",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders",
            column: "CustomerId",
            principalTable: "Customers",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders");

        migrationBuilder.DropColumn(
            name: "PublishDate",
            table: "Games");

        migrationBuilder.AlterColumn<int>(
            name: "CustomerId",
            table: "Orders",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Orders_Customers_CustomerId",
            table: "Orders",
            column: "CustomerId",
            principalTable: "Customers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
