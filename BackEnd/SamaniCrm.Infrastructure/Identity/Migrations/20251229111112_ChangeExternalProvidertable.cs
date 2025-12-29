using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class ChangeExternalProvidertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "ExternalProviders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "ExternalProviders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ExternalProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "ExternalProviders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ExternalProviders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedTime",
                table: "ExternalProviders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoutEndpoint",
                table: "ExternalProviders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseMode",
                table: "ExternalProviders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponseType",
                table: "ExternalProviders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "UsePkce",
                table: "ExternalProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "LastModifiedTime",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "LogoutEndpoint",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "ResponseMode",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "ResponseType",
                table: "ExternalProviders");

            migrationBuilder.DropColumn(
                name: "UsePkce",
                table: "ExternalProviders");
        }
    }
}
