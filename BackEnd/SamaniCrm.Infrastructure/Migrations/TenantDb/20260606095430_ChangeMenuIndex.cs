using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class ChangeMenuIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Menus_Name",
                table: "Menus");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Name_TenantId",
                table: "Menus",
                columns: new[] { "Name", "TenantId" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Menus_Name_TenantId",
                table: "Menus");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Name",
                table: "Menus",
                column: "Name",
                unique: true);
        }
    }
}
