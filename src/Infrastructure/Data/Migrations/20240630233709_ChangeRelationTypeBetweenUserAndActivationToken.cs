using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFridgeManagerAPI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRelationTypeBetweenUserAndActivationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivationTokens_UserId",
                table: "ActivationTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ResetPasswordTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ActivationTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivationTokens_UserId",
                table: "ActivationTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivationTokens_UserId",
                table: "ActivationTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ResetPasswordTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "ActivationTokens",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationTokens_UserId",
                table: "ActivationTokens",
                column: "UserId",
                unique: true);
        }
    }
}
