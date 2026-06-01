using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLogEntryToAppLogEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "LogSettings",
                schema: "logs");

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                schema: "logs",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "TenantAppLogSettings",
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
                    table.PrimaryKey("PK_TenantAppLogSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppLogEntries",
                schema: "logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AppLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLogEntries_TenantAppLogSettings_TenantSettingId",
                        column: x => x.TenantSettingId,
                        principalSchema: "logs",
                        principalTable: "TenantAppLogSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_CorrelationId",
                schema: "logs",
                table: "AppLogEntries",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_Level",
                schema: "logs",
                table: "AppLogEntries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_TenantId",
                schema: "logs",
                table: "AppLogEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_TenantId_Timestamp_Level",
                schema: "logs",
                table: "AppLogEntries",
                columns: new[] { "TenantId", "Timestamp", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_TenantSettingId",
                schema: "logs",
                table: "AppLogEntries",
                column: "TenantSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_Timestamp",
                schema: "logs",
                table: "AppLogEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_UserId",
                schema: "logs",
                table: "AppLogEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantAppLogSettings_TenantId",
                schema: "logs",
                table: "TenantAppLogSettings",
                column: "TenantId",
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogEntries",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "TenantAppLogSettings",
                schema: "logs");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                schema: "logs",
                table: "AuditLogs");

            migrationBuilder.CreateTable(
                name: "LogSettings",
                schema: "logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomSettings = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EnabledLevels = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnabledSinks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetentionDays = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    TenantSettingId = table.Column<int>(type: "int", nullable: true),
                    ActionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ControllerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CorrelationId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExceptionDetails = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                    ExtraData = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RequestPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
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
    }
}
