using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdentityTenantTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSetting_Users_UserId",
                table: "UserSetting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "Tenant");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "idnty",
                newName: "UserTokens",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "idnty",
                newName: "Users",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "idnty",
                newName: "UserRoles",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "idnty",
                newName: "UserLogins",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "idnty",
                newName: "UserClaims",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "TenantSettings",
                newName: "TenantSettings",
                newSchema: "Tenant");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "Tenants",
                newSchema: "Tenant");

            migrationBuilder.RenameTable(
                name: "TenantProvisioningSteps",
                newName: "TenantProvisioningSteps",
                newSchema: "Tenant");

            migrationBuilder.RenameTable(
                name: "TenantDatabaseConnections",
                newName: "TenantDatabaseConnections",
                newSchema: "Tenant");

            migrationBuilder.RenameTable(
                name: "TenantCategories",
                newName: "TenantCategories",
                newSchema: "Tenant");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "idnty",
                newName: "Roles",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "idnty",
                newName: "RolePermissions",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "idnty",
                newName: "RoleClaims",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshTokens",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "idnty",
                newName: "Permissions",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "UserSetting",
                newName: "UserSettings",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UserSetting_UserId",
                schema: "auth",
                table: "UserSettings",
                newName: "IX_UserSettings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSettings",
                schema: "auth",
                table: "UserSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSettings_Users_UserId",
                schema: "auth",
                table: "UserSettings",
                column: "UserId",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSettings_Users_UserId",
                schema: "auth",
                table: "UserSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSettings",
                schema: "auth",
                table: "UserSettings");

            migrationBuilder.EnsureSchema(
                name: "idnty");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "auth",
                newName: "UserTokens",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "auth",
                newName: "Users",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "auth",
                newName: "UserRoles",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "auth",
                newName: "UserLogins",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "auth",
                newName: "UserClaims",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "TenantSettings",
                schema: "Tenant",
                newName: "TenantSettings");

            migrationBuilder.RenameTable(
                name: "Tenants",
                schema: "Tenant",
                newName: "Tenants");

            migrationBuilder.RenameTable(
                name: "TenantProvisioningSteps",
                schema: "Tenant",
                newName: "TenantProvisioningSteps");

            migrationBuilder.RenameTable(
                name: "TenantDatabaseConnections",
                schema: "Tenant",
                newName: "TenantDatabaseConnections");

            migrationBuilder.RenameTable(
                name: "TenantCategories",
                schema: "Tenant",
                newName: "TenantCategories");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "auth",
                newName: "Roles",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "auth",
                newName: "RolePermissions",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "auth",
                newName: "RoleClaims",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "auth",
                newName: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "auth",
                newName: "Permissions",
                newSchema: "idnty");

            migrationBuilder.RenameTable(
                name: "UserSettings",
                schema: "auth",
                newName: "UserSetting");

            migrationBuilder.RenameIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSetting",
                newName: "IX_UserSetting_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSetting_Users_UserId",
                table: "UserSetting",
                column: "UserId",
                principalSchema: "idnty",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
