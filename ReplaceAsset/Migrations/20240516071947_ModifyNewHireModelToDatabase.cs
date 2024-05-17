using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNewHireModelToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdaptorGiven",
                table: "NewHire",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BagGiven",
                table: "NewHire",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HeadsetGiven",
                table: "NewHire",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LaptopGiven",
                table: "NewHire",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PowerCableGiven",
                table: "NewHire",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdaptorGiven",
                table: "NewHire");

            migrationBuilder.DropColumn(
                name: "BagGiven",
                table: "NewHire");

            migrationBuilder.DropColumn(
                name: "HeadsetGiven",
                table: "NewHire");

            migrationBuilder.DropColumn(
                name: "LaptopGiven",
                table: "NewHire");

            migrationBuilder.DropColumn(
                name: "PowerCableGiven",
                table: "NewHire");
        }
    }
}
