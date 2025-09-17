using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class changeLocalizationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Localizations_Key_Culture",
                table: "Localizations");

            migrationBuilder.CreateIndex(
                name: "IX_Localizations_Key_Culture_Category",
                table: "Localizations",
                columns: new[] { "Key", "Culture", "Category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Localizations_Key_Culture_Category",
                table: "Localizations");

            migrationBuilder.CreateIndex(
                name: "IX_Localizations_Key_Culture",
                table: "Localizations",
                columns: new[] { "Key", "Culture" },
                unique: true);
        }
    }
}
