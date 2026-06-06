using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations.TenantDb
{
    /// <inheritdoc />
    public partial class RoleChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("""
            DROP INDEX IF EXISTS [RoleNameIndex]
            ON [auth].[Roles];
            """);


            migrationBuilder.Sql("""
            DROP INDEX IF EXISTS [IX_Roles_Id_Name_TenantId]
            ON [auth].[Roles];
            """);


            //migrationBuilder.DropIndex(
            //       name: "RoleNameIndex",
            //       schema: "auth",
            //       table: "Roles");

            //migrationBuilder.DropIndex(
            //    name: "IX_Roles_Id_Name_TenantId",
            //    schema: "auth",
            //    table: "Roles");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Roles_Name_TenantId",
            //    schema: "auth",
            //    table: "Roles",
            //    columns: new[] { "Name", "TenantId" },
            //    unique: true,
            //    filter: "[Name] IS NOT NULL AND [TenantId] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Roles_TenantId",
            //    schema: "auth",
            //    table: "Roles",
            //    column: "TenantId");
            migrationBuilder.Sql("""
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Roles_Name_TenantId'
      AND object_id = OBJECT_ID('auth.Roles')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_Name_TenantId]
    ON [auth].[Roles] ([Name], [TenantId])
    WHERE [Name] IS NOT NULL
      AND [TenantId] IS NOT NULL
END
""");

            migrationBuilder.Sql("""
IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Roles_TenantId'
      AND object_id = OBJECT_ID('auth.Roles')
)
BEGIN
    CREATE INDEX [IX_Roles_TenantId]
    ON [auth].[Roles] ([TenantId])
END
""");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
