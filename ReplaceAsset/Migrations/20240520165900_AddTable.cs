using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAdmin",
                table: "UserAdmin");

            migrationBuilder.RenameTable(
                name: "UserAdmin",
                newName: "UserAdmins");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins");

            migrationBuilder.RenameTable(
                name: "UserAdmins",
                newName: "UserAdmin");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAdmin",
                table: "UserAdmin",
                column: "Id");
        }
    }
}
