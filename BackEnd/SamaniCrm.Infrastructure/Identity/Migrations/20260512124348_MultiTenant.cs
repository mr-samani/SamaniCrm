using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTypeTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributeTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PageTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "MenuTranslations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Localizations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "file",
                table: "FileFolders");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Product",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "product",
                table: "Currencies");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductTypes",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductTypes",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductTypes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "Products",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "Products",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "Products",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductPrices",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductPrices",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductPrices",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductImages",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductImages",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductImages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductFiles",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductFiles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductFiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductCategories",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductCategories",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductCategories",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "ProductAttributes",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "ProductAttributes",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "ProductAttributes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "PgB",
                table: "Plugins",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "PgB",
                table: "Plugins",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "PageTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "PageTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "PageTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "Pages",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "Pages",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Pages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "Notifications",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "Notifications",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "MenuTranslations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "MenuTranslations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "MenuTranslations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "Menus",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "Menus",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Menus",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "Localizations",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "Localizations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Localizations",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "Languages",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                table: "Languages",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Languages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "file",
                table: "FileFolders",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "file",
                table: "FileFolders",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "file",
                table: "FileFolders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                table: "ExternalProviders",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "ExternalProviders",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "Product",
                table: "Discounts",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "Product",
                table: "Discounts",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "Product",
                table: "Discounts",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "panel",
                table: "Dashboards",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "panel",
                table: "Dashboards",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "panel",
                table: "DashboardItems",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "panel",
                table: "DashboardItems",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifiedTime",
                schema: "product",
                table: "Currencies",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "DeletedTime",
                schema: "product",
                table: "Currencies",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "product",
                table: "Currencies",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                schema: "product",
                table: "Carts",
                newName: "ModifiedAt");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductTypes",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "Products",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductPrices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductPrices",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductImages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductFiles",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductCategories",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributeValues",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributes",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "PgB",
                table: "Plugins",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "PgB",
                table: "Plugins",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "PgB",
                table: "Plugins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "PgB",
                table: "Plugins",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "PgB",
                table: "Plugins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "PgB",
                table: "Plugins",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "PageTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "PageTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "PageTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PageTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Pages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Pages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Pages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Pages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Notifications",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "MenuTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "MenuTranslations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "MenuTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "MenuTranslations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Menus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Menus",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Localizations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Localizations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Localizations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Localizations",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                table: "Languages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Languages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Languages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "Languages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Languages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "file",
                table: "FileFolders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "file",
                table: "FileFolders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "file",
                table: "FileFolders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "file",
                table: "FileFolders",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "ExternalProviders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ExternalProviders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ExternalProviders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ExternalProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "ExternalProviders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ExternalProviders",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "Product",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "Product",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "Product",
                table: "Discounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Product",
                table: "Discounts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "panel",
                table: "Dashboards",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "panel",
                table: "Dashboards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "panel",
                table: "Dashboards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "panel",
                table: "Dashboards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "panel",
                table: "Dashboards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "panel",
                table: "Dashboards",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "panel",
                table: "DashboardItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "panel",
                table: "DashboardItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "panel",
                table: "DashboardItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "panel",
                table: "DashboardItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "panel",
                table: "DashboardItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "panel",
                table: "DashboardItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "Currencies",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "Currencies",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "Currencies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "Currencies",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "product",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "product",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "Carts",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "product",
                table: "CartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "product",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                schema: "product",
                table: "CartItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "product",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "product",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "product",
                table: "CartItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                schema: "product",
                table: "CartItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "product",
                table: "CartItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    DatabaseStrategy = table.Column<int>(type: "int", nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatabaseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaviconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionPlan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaxUsers = table.Column<int>(type: "int", nullable: false),
                    MaxStorageMB = table.Column<long>(type: "bigint", nullable: false),
                    MaxApiCallsPerMonth = table.Column<long>(type: "bigint", nullable: true),
                    CurrentStorageMB = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsTrial = table.Column<bool>(type: "bit", nullable: false),
                    TrialEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionStartsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProvisioningStatus = table.Column<int>(type: "int", nullable: false),
                    ProvisioningError = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowedIpAddresses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Require2FA = table.Column<bool>(type: "bit", nullable: false),
                    SessionTimeoutMinutes = table.Column<int>(type: "int", nullable: false),
                    PasswordMinLength = table.Column<int>(type: "int", nullable: false),
                    PasswordRequireSpecialChar = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuspensionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProvisioningSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisioningSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisioningSteps_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TenantCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantCategories_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantDatabaseConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatabaseType = table.Column<int>(type: "int", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EncryptedPassword = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMaster = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastHealthCheck = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HealthStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDatabaseConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantDatabaseConnections_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValueType = table.Column<int>(type: "int", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProvisioningSteps_TenantId",
                table: "ProvisioningSteps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCategories_TenantId",
                table: "TenantCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantDatabaseConnections_TenantId",
                table: "TenantDatabaseConnections",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvisioningSteps");

            migrationBuilder.DropTable(
                name: "TenantCategories");

            migrationBuilder.DropTable(
                name: "TenantDatabaseConnections");

            migrationBuilder.DropTable(
                name: "TenantSettings");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTypeTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductTypeTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductPrices");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributeTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributeTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "PgB",
                table: "Plugins");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "PageTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PageTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "MenuTranslations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "MenuTranslations");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Localizations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Localizations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "file",
                table: "FileFolders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "file",
                table: "FileFolders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "Product",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Product",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "panel",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "panel",
                table: "DashboardItems");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "product",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "product",
                table: "CartItems");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductTypeTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductTypes",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductTypes",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductTypes",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "Products",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "Products",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "Products",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductPrices",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductPrices",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductPrices",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductImages",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductImages",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductImages",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductFiles",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductFiles",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductFiles",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductCategories",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductCategories",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductCategories",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductAttributeValues",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductAttributeTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "ProductAttributes",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "ProductAttributes",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "ProductAttributes",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "PgB",
                table: "Plugins",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "PgB",
                table: "Plugins",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "PageTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "PageTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PageTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Pages",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Pages",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Pages",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Notifications",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Notifications",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Notifications",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "MenuTranslations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "MenuTranslations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "MenuTranslations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Menus",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Menus",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Menus",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Localizations",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Localizations",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Localizations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Languages",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Languages",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Languages",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "file",
                table: "FileFolders",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "file",
                table: "FileFolders",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "file",
                table: "FileFolders",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "ExternalProviders",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ExternalProviders",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "Product",
                table: "Discounts",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "Product",
                table: "Discounts",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "Product",
                table: "Discounts",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "panel",
                table: "Dashboards",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "panel",
                table: "Dashboards",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "panel",
                table: "DashboardItems",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "panel",
                table: "DashboardItems",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "Currencies",
                newName: "LastModifiedTime");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                schema: "product",
                table: "Currencies",
                newName: "DeletedTime");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "product",
                table: "Currencies",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                schema: "product",
                table: "Carts",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTypeTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductPrices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductPrices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductPrices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductCategoryTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributeValues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributeTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "PgB",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "PgB",
                table: "Plugins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "PageTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PageTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "PageTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "MenuTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "MenuTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "MenuTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Localizations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Localizations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Localizations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Languages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "file",
                table: "FileFolders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "file",
                table: "FileFolders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "file",
                table: "FileFolders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ExternalProviders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ExternalProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "Product",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "Product",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Product",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "panel",
                table: "Dashboards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "panel",
                table: "Dashboards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "panel",
                table: "DashboardItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "panel",
                table: "DashboardItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeletedBy",
                schema: "product",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "product",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "product",
                table: "Currencies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
