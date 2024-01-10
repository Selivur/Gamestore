using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gamestore.Api.Migrations;

/// <inheritdoc />
public partial class UpdateOrderAndOrderDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Customers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Customers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Genres",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ParentId = table.Column<int>(type: "int", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Genres", x => x.Id);
                table.ForeignKey(
                    name: "FK_Genres_Genres_ParentId",
                    column: x => x.ParentId,
                    principalTable: "Genres",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Platforms",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Platforms", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Publishers",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CompanyName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                HomePage = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Publishers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                OrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Price = table.Column<int>(type: "int", nullable: false),
                CustomerId = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
                table.ForeignKey(
                    name: "FK_Orders_Customers_CustomerId",
                    column: x => x.CustomerId,
                    principalTable: "Customers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                GameAlias = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Price = table.Column<int>(type: "int", nullable: false),
                UnitInStock = table.Column<int>(type: "int", nullable: false),
                Discount = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PublishersId = table.Column<int>(type: "int", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
                table.ForeignKey(
                    name: "FK_Games_Publishers_PublishersId",
                    column: x => x.PublishersId,
                    principalTable: "Publishers",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "GameGenre",
            columns: table => new
            {
                GamesId = table.Column<int>(type: "int", nullable: false),
                GenreId = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GameGenre", x => new { x.GamesId, x.GenreId });
                table.ForeignKey(
                    name: "FK_GameGenre_Games_GamesId",
                    column: x => x.GamesId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GameGenre_Genres_GenreId",
                    column: x => x.GenreId,
                    principalTable: "Genres",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GamePlatform",
            columns: table => new
            {
                GamesId = table.Column<int>(type: "int", nullable: false),
                PlatformsId = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GamePlatform", x => new { x.GamesId, x.PlatformsId });
                table.ForeignKey(
                    name: "FK_GamePlatform_Games_GamesId",
                    column: x => x.GamesId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GamePlatform_Platforms_PlatformsId",
                    column: x => x.PlatformsId,
                    principalTable: "Platforms",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "OrderDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Quantity = table.Column<int>(type: "int", nullable: false),
                Price = table.Column<int>(type: "int", nullable: false),
                Discount = table.Column<int>(type: "int", nullable: false),
                GameId = table.Column<int>(type: "int", nullable: false),
                OrderId = table.Column<int>(type: "int", nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderDetails", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderDetails_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_OrderDetails_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_GameGenre_GenreId",
            table: "GameGenre",
            column: "GenreId");

        migrationBuilder.CreateIndex(
            name: "IX_GamePlatform_PlatformsId",
            table: "GamePlatform",
            column: "PlatformsId");

        migrationBuilder.CreateIndex(
            name: "IX_Games_GameAlias",
            table: "Games",
            column: "GameAlias",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Games_PublishersId",
            table: "Games",
            column: "PublishersId");

        migrationBuilder.CreateIndex(
            name: "IX_Genres_Name",
            table: "Genres",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Genres_ParentId",
            table: "Genres",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_OrderDetails_GameId",
            table: "OrderDetails",
            column: "GameId");

        migrationBuilder.CreateIndex(
            name: "IX_OrderDetails_OrderId",
            table: "OrderDetails",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_Orders_CustomerId",
            table: "Orders",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_Platforms_Type",
            table: "Platforms",
            column: "Type",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Publishers_CompanyName",
            table: "Publishers",
            column: "CompanyName",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "GameGenre");

        migrationBuilder.DropTable(
            name: "GamePlatform");

        migrationBuilder.DropTable(
            name: "OrderDetails");

        migrationBuilder.DropTable(
            name: "Genres");

        migrationBuilder.DropTable(
            name: "Platforms");

        migrationBuilder.DropTable(
            name: "Games");

        migrationBuilder.DropTable(
            name: "Orders");

        migrationBuilder.DropTable(
            name: "Publishers");

        migrationBuilder.DropTable(
            name: "Customers");
    }
}
