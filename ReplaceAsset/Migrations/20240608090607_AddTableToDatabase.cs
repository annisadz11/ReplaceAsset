﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReplaceAsset.Migrations
{
    /// <inheritdoc />
    public partial class AddTableToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "IT-App_Schema");

            migrationBuilder.CreateTable(
                name: "AssetRequest",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    TypeReplace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetScrap",
                schema: "IT-App_Schema",
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
                name: "NewHire",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelAsset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfJoin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusCompleted = table.Column<bool>(type: "bit", nullable: false),
                    HeadsetGiven = table.Column<bool>(type: "bit", nullable: false),
                    LaptopGiven = table.Column<bool>(type: "bit", nullable: false),
                    AdaptorGiven = table.Column<bool>(type: "bit", nullable: false),
                    PowerCableGiven = table.Column<bool>(type: "bit", nullable: false),
                    BagGiven = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewHire", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAdmins",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdmins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInterns",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserManagerITs",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManagerITs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentAssetReplacement",
                schema: "IT-App_Schema",
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
                        principalSchema: "IT-App_Schema",
                        principalTable: "AssetRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewAssetReplacement",
                schema: "IT-App_Schema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetRequestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReplace = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewAssetReplacement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewAssetReplacement_AssetRequest_AssetRequestId",
                        column: x => x.AssetRequestId,
                        principalSchema: "IT-App_Schema",
                        principalTable: "AssetRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentAssetReplacement_AssetRequestId",
                schema: "IT-App_Schema",
                table: "ComponentAssetReplacement",
                column: "AssetRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewAssetReplacement_AssetRequestId",
                schema: "IT-App_Schema",
                table: "NewAssetReplacement",
                column: "AssetRequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetScrap",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "ComponentAssetReplacement",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "NewAssetReplacement",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "NewHire",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "UserAdmins",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "UserInterns",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "UserManagerITs",
                schema: "IT-App_Schema");

            migrationBuilder.DropTable(
                name: "AssetRequest",
                schema: "IT-App_Schema");
        }
    }
}
