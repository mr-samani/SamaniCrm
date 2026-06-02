using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                schema: "auth",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_ApplicationUserId",
                schema: "auth",
                table: "Roles",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Users_ApplicationUserId",
                schema: "auth",
                table: "Roles",
                column: "ApplicationUserId",
                principalSchema: "auth",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Users_ApplicationUserId",
                schema: "auth",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_ApplicationUserId",
                schema: "auth",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "auth",
                table: "Roles");
        }
    }
}
