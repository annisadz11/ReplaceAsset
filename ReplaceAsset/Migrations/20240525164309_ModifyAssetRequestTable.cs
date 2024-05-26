using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAssetRequestTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailUser",
                table: "AssetRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailUser",
                table: "AssetRequest");
        }
    }
}
