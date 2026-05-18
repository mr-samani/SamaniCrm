using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Logging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "logs");

            migrationBuilder.CreateTable(
                name: "LogSettings",
                schema: "logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EnabledLevels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnabledSinks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RetentionDays = table.Column<int>(type: "int", nullable: true),
                    CustomSettings = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogEntries",
                schema: "logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ExceptionDetails = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ControllerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantSettingId = table.Column<int>(type: "int", nullable: true),
                    ExtraData = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogEntries_LogSettings_TenantSettingId",
                        column: x => x.TenantSettingId,
                        principalSchema: "logs",
                        principalTable: "LogSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_CorrelationId",
                schema: "logs",
                table: "LogEntries",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Level",
                schema: "logs",
                table: "LogEntries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_TenantId",
                schema: "logs",
                table: "LogEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_TenantId_Timestamp_Level",
                schema: "logs",
                table: "LogEntries",
                columns: new[] { "TenantId", "Timestamp", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_TenantSettingId",
                schema: "logs",
                table: "LogEntries",
                column: "TenantSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Timestamp",
                schema: "logs",
                table: "LogEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_UserId",
                schema: "logs",
                table: "LogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogSettings_TenantId",
                schema: "logs",
                table: "LogSettings",
                column: "TenantId",
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "LogSettings",
                schema: "logs");
        }
    }
}
