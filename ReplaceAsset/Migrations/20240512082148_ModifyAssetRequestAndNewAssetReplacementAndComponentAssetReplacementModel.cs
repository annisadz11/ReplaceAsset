using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAssetRequestAndNewAssetReplacementAndComponentAssetReplacementModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                table: "NewAssetReplacement");

            migrationBuilder.DropIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                table: "ComponentAssetReplacement");

            migrationBuilder.AlterColumn<string>(
                name: "NewType",
                table: "NewAssetReplacement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewSerialNumber",
                table: "NewAssetReplacement",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                table: "NewAssetReplacement",
                column: "AssetRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                table: "ComponentAssetReplacement",
                column: "AssetRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                table: "NewAssetReplacement");

            migrationBuilder.DropIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                table: "ComponentAssetReplacement");

            migrationBuilder.AlterColumn<string>(
                name: "NewType",
                table: "NewAssetReplacement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NewSerialNumber",
                table: "NewAssetReplacement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                table: "NewAssetReplacement",
                column: "AssetRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                table: "ComponentAssetReplacement",
                column: "AssetRequestId");
        }
    }
}
