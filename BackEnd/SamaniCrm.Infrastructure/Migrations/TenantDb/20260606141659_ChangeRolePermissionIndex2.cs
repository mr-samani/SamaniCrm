using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class ChangeRolePermissionIndex2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "auth",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId_TenantId",
                schema: "auth",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                schema: "auth",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "auth",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "auth",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId_TenantId",
                schema: "auth",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "TenantId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                schema: "auth",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId_TenantId",
                schema: "auth",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "auth",
                table: "RolePermissions");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                schema: "auth",
                table: "RolePermissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                schema: "auth",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId_TenantId",
                schema: "auth",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId", "TenantId" },
                unique: true);
        }
    }
}
