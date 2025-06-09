using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddFileIdForProductFilesAndImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "FileType",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                schema: "product",
                table: "ProductImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                schema: "product",
                table: "ProductFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProductImages_FileId",
                schema: "product",
                table: "ProductImages",
                column: "FileId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ProductFiles_FileId",
                schema: "product",
                table: "ProductFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_FileId",
                schema: "product",
                table: "ProductImages",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFiles_FileId",
                schema: "product",
                table: "ProductFiles",
                column: "FileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFiles_FileFolders_FileId",
                schema: "product",
                table: "ProductFiles",
                column: "FileId",
                principalSchema: "file",
                principalTable: "FileFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_FileFolders_FileId",
                schema: "product",
                table: "ProductImages",
                column: "FileId",
                principalSchema: "file",
                principalTable: "FileFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFiles_FileFolders_FileId",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_FileFolders_FileId",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProductImages_FileId",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_FileId",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ProductFiles_FileId",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropIndex(
                name: "IX_ProductFiles_FileId",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "FileId",
                schema: "product",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.DropColumn(
                name: "FileId",
                schema: "product",
                table: "ProductFiles");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                schema: "product",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                schema: "product",
                table: "ProductFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
