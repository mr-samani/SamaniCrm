using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndedFromIp",
                schema: "auth",
                table: "UserDelegations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartedFromIp",
                schema: "auth",
                table: "UserDelegations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndedFromIp",
                schema: "auth",
                table: "UserDelegations");

            migrationBuilder.DropColumn(
                name: "StartedFromIp",
                schema: "auth",
                table: "UserDelegations");
        }
    }
}
