using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class ExternalProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalProviders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Scheme = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderType = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    AuthorizationEndpoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TokenEndpoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserInfoEndpoint = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CallbackPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scopes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProviders", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalProviders");
        }
    }
}
