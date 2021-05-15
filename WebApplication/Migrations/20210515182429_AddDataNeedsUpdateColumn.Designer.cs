﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WebApplication.Infrastructure;

namespace WebApplication.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210515182429_AddDataNeedsUpdateColumn")]
    partial class AddDataNeedsUpdateColumn
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MobileTracking.Core.Models.Calibration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PositionId")
                        .HasColumnType("integer");

                    b.Property<string>("SignalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SignalType")
                        .HasColumnType("integer");

                    b.Property<float>("Strength")
                        .HasColumnType("real");

                    b.Property<float>("X")
                        .HasColumnType("real");

                    b.Property<float>("Y")
                        .HasColumnType("real");

                    b.Property<float>("Z")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("PositionId");

                    b.ToTable("Calibrations");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Locale", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Locales");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.LocalizationMeasurement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SignalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SignalType")
                        .HasColumnType("integer");

                    b.Property<float>("Strength")
                        .HasColumnType("real");

                    b.Property<int>("UserLocalizationId")
                        .HasColumnType("integer");

                    b.Property<float>("X")
                        .HasColumnType("real");

                    b.Property<float>("Y")
                        .HasColumnType("real");

                    b.Property<float>("Z")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("UserLocalizationId");

                    b.ToTable("LocalizationMeasurements");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("DataNeedsUpdate")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("X")
                        .HasColumnType("real");

                    b.Property<float>("Y")
                        .HasColumnType("real");

                    b.Property<float>("Z")
                        .HasColumnType("real");

                    b.Property<int>("ZoneId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ZoneId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.PositionData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastUpdate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<float>("Max")
                        .HasColumnType("real");

                    b.Property<float>("MaxX")
                        .HasColumnType("real");

                    b.Property<float>("MaxY")
                        .HasColumnType("real");

                    b.Property<float>("MaxZ")
                        .HasColumnType("real");

                    b.Property<float>("Min")
                        .HasColumnType("real");

                    b.Property<float>("MinX")
                        .HasColumnType("real");

                    b.Property<float>("MinY")
                        .HasColumnType("real");

                    b.Property<float>("MinZ")
                        .HasColumnType("real");

                    b.Property<int>("PositionId")
                        .HasColumnType("integer");

                    b.Property<int>("Samples")
                        .HasColumnType("integer");

                    b.Property<string>("SignalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SignalType")
                        .HasColumnType("integer");

                    b.Property<float>("StandardDeviation")
                        .HasColumnType("real");

                    b.Property<float>("StandardDeviationX")
                        .HasColumnType("real");

                    b.Property<float>("StandardDeviationY")
                        .HasColumnType("real");

                    b.Property<float>("StandardDeviationZ")
                        .HasColumnType("real");

                    b.Property<float>("Strength")
                        .HasColumnType("real");

                    b.Property<float>("X")
                        .HasColumnType("real");

                    b.Property<float>("Y")
                        .HasColumnType("real");

                    b.Property<float>("Z")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("PositionId");

                    b.ToTable("PositionsData");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.UserLocalization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CalculatedPositionId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CalculatedPositionId");

                    b.ToTable("UserLocalizations");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Zone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("Floor")
                        .HasColumnType("integer");

                    b.Property<int>("LocaleId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LocaleId");

                    b.ToTable("Zones");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Calibration", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.Position", "Position")
                        .WithMany("Calibrations")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Position");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.LocalizationMeasurement", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.UserLocalization", null)
                        .WithMany("LocalizationMeasurements")
                        .HasForeignKey("UserLocalizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Position", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.Zone", "Zone")
                        .WithMany("Positions")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.PositionData", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.Position", "Position")
                        .WithMany("PositionData")
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Position");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.UserLocalization", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.Position", "CalculatedPosition")
                        .WithMany()
                        .HasForeignKey("CalculatedPositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CalculatedPosition");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Zone", b =>
                {
                    b.HasOne("MobileTracking.Core.Models.Locale", "Locale")
                        .WithMany("Zones")
                        .HasForeignKey("LocaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Locale");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Locale", b =>
                {
                    b.Navigation("Zones");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Position", b =>
                {
                    b.Navigation("Calibrations");

                    b.Navigation("PositionData");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.UserLocalization", b =>
                {
                    b.Navigation("LocalizationMeasurements");
                });

            modelBuilder.Entity("MobileTracking.Core.Models.Zone", b =>
                {
                    b.Navigation("Positions");
                });
#pragma warning restore 612, 618
        }
    }
}
