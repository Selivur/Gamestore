using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gamestore.Api.Migrations;

/// <inheritdoc />
public partial class AddedMongoFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProductCategories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductCategories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProductOrders",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EmployeeID = table.Column<int>(type: "int", nullable: false),
                OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                ShippedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ShipVia = table.Column<int>(type: "int", nullable: false),
                Freight = table.Column<double>(type: "float", nullable: false),
                ShipName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShipAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShipCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShipRegion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShipPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShipCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductOrders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProductShippers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductShippers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProductSuppliers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ContactTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Fax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                HomePage = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductSuppliers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                SupplierID = table.Column<int>(type: "int", nullable: false),
                CategoryID = table.Column<int>(type: "int", nullable: false),
                QuantityPerUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                UnitsInStock = table.Column<int>(type: "int", nullable: false),
                UnitsOnOrder = table.Column<int>(type: "int", nullable: false),
                ReorderLevel = table.Column<int>(type: "int", nullable: false),
                Discontinued = table.Column<bool>(type: "bit", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_ProductCategories_CategoryID",
                    column: x => x.CategoryID,
                    principalTable: "ProductCategories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Products_ProductSuppliers_SupplierID",
                    column: x => x.SupplierID,
                    principalTable: "ProductSuppliers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProductOrderDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProductID = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductOrderDetails", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductOrderDetails_Products_ProductID",
                    column: x => x.ProductID,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProductOrderDetails_ProductID",
            table: "ProductOrderDetails",
            column: "ProductID");

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryID",
            table: "Products",
            column: "CategoryID");

        migrationBuilder.CreateIndex(
            name: "IX_Products_SupplierID",
            table: "Products",
            column: "SupplierID");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ProductOrderDetails");

        migrationBuilder.DropTable(
            name: "ProductOrders");

        migrationBuilder.DropTable(
            name: "ProductShippers");

        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.DropTable(
            name: "ProductCategories");

        migrationBuilder.DropTable(
            name: "ProductSuppliers");
    }
}
