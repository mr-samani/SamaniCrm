using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SamaniCrm.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class FixupTenantProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "TenantProvisioningSteps");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TenantProvisioningSteps",
                newName: "StepStatus");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TenantProvisioningSteps",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "TenantProvisioningSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "TenantProvisioningSteps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "TenantProvisioningSteps");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "TenantProvisioningSteps");

            migrationBuilder.RenameColumn(
                name: "StepStatus",
                table: "TenantProvisioningSteps",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TenantProvisioningSteps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TenantProvisioningSteps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
