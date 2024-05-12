using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Departement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Baseline = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsageLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Justify = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeReplace = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetScrap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateInput = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidationScrap = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetScrap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentAssetReplacement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetRequestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationReplace = table.Column<bool>(type: "bit", nullable: false),
                    ComponentReplaceDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentAssetReplacement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentAssetReplacement_AssetRequest_AssetRequestId",
                        column: x => x.AssetRequestId,
                        principalTable: "AssetRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewAssetReplacement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetRequestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateReplace = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewAssetReplacement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewAssetReplacement_AssetRequest_AssetRequestId",
                        column: x => x.AssetRequestId,
                        principalTable: "AssetRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                table: "ComponentAssetReplacement",
                column: "AssetRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                table: "NewAssetReplacement",
                column: "AssetRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetScrap");

            migrationBuilder.DropTable(
                name: "ComponentAssetReplacement");

            migrationBuilder.DropTable(
                name: "NewAssetReplacement");

            migrationBuilder.DropTable(
                name: "AssetRequest");
        }
    }
}
