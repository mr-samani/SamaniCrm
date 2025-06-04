using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyAndDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                schema: "product",
                table: "ProductPrices",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "product",
                table: "ProductPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExchangeRateToBase = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.UniqueConstraint("AK_Currencies_CurrencyCode", x => x.CurrencyCode);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expired = table.Column<bool>(type: "bit", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrices_CurrencyCode",
                schema: "product",
                table: "ProductPrices",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CurrencyCode",
                schema: "product",
                table: "Currencies",
                column: "CurrencyCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPrices_Currencies_CurrencyCode",
                schema: "product",
                table: "ProductPrices",
                column: "CurrencyCode",
                principalSchema: "product",
                principalTable: "Currencies",
                principalColumn: "CurrencyCode",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPrices_Currencies_CurrencyCode",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "product");

            migrationBuilder.DropTable(
                name: "Discounts",
                schema: "Product");

            migrationBuilder.DropIndex(
                name: "IX_ProductPrices_CurrencyCode",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "product",
                table: "ProductPrices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
