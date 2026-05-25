using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantIdToTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "SecuritySettings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "identity",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributeValues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "PgB",
                table: "Plugins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "PageTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Pages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "MenuTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "file",
                table: "FileFolders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Product",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "panel",
                table: "Dashboards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "panel",
                table: "DashboardItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "Currencies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "product",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SecuritySettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "identity",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductTypeTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributeTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PageTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MenuTranslations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "file",
                table: "FileFolders");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Product",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "product",
                table: "CartItems");
        }
    }
}
