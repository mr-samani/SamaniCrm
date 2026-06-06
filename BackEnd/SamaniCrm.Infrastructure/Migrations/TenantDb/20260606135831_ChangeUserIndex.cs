using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class ChangeUserIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
              name: "UserNameIndex",
              schema: "auth",
              table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                schema: "auth",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_UserName",
                schema: "auth",
                table: "Users",
                columns: new[] { "TenantId", "UserName" },
                unique: true,
                filter: "[TenantId] IS NOT NULL AND [UserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                schema: "auth",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId_UserName",
                schema: "auth",
                table: "Users");
        }
    }
}
