using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProcustCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                schema: "product",
                table: "ProductCategories",
                newName: "OrderIndex");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                schema: "product",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                schema: "product",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "product",
                table: "ProductCategoryTranslations",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OrderIndex",
                schema: "product",
                table: "ProductCategories",
                newName: "SortOrder");
        }
    }
}
