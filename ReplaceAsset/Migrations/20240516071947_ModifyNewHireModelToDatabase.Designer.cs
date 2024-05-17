﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReplaceAsset.Data;

#nullable disable

namespace ReplaceAsset.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240516071947_ModifyNewHireModelToDatabase")]
    partial class ModifyNewHireModelToDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ReplaceAsset.Models.AssetRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Baseline")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Departement")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Justify")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RequestDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeReplace")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsageLocation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AssetRequest");
                });

            modelBuilder.Entity("ReplaceAsset.Models.AssetScrap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DateInput")
                        .HasColumnType("datetime2");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ValidationScrap")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("AssetScrap");
                });

            modelBuilder.Entity("ReplaceAsset.Models.ComponentAssetReplacement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetRequestId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ComponentReplaceDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ValidationReplace")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AssetRequestId")
                        .IsUnique();

                    b.ToTable("ComponentAssetReplacement");
                });

            modelBuilder.Entity("ReplaceAsset.Models.NewAssetReplacement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssetRequestId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateReplace")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewSerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NewType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssetRequestId")
                        .IsUnique();

                    b.ToTable("NewAssetReplacement");
                });

            modelBuilder.Entity("ReplaceAsset.Models.NewHire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AdaptorGiven")
                        .HasColumnType("bit");

                    b.Property<bool>("BagGiven")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("DateOfJoin")
                        .HasColumnType("datetime2");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Designation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Device")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HeadsetGiven")
                        .HasColumnType("bit");

                    b.Property<bool>("LaptopGiven")
                        .HasColumnType("bit");

                    b.Property<string>("ModelAsset")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PowerCableGiven")
                        .HasColumnType("bit");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("StatusCompleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("NewHire");
                });

            modelBuilder.Entity("ReplaceAsset.Models.ComponentAssetReplacement", b =>
                {
                    b.HasOne("ReplaceAsset.Models.AssetRequest", "AssetRequest")
                        .WithOne("ComponentAssetReplacement")
                        .HasForeignKey("ReplaceAsset.Models.ComponentAssetReplacement", "AssetRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssetRequest");
                });

            modelBuilder.Entity("ReplaceAsset.Models.NewAssetReplacement", b =>
                {
                    b.HasOne("ReplaceAsset.Models.AssetRequest", "AssetRequest")
                        .WithOne("NewAssetReplacement")
                        .HasForeignKey("ReplaceAsset.Models.NewAssetReplacement", "AssetRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssetRequest");
                });

            modelBuilder.Entity("ReplaceAsset.Models.AssetRequest", b =>
                {
                    b.Navigation("ComponentAssetReplacement");

                    b.Navigation("NewAssetReplacement");
                });
#pragma warning restore 612, 618
        }
    }
}
