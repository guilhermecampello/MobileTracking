using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApplication.Migrations
{
    public partial class AddBaseEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Latitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocaleId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Floor = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zones_Locales_LocaleId",
                        column: x => x.LocaleId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    XCoordinate = table.Column<float>(type: "real", nullable: false),
                    YCoordinate = table.Column<float>(type: "real", nullable: false),
                    ZCoordinate = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calibrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PositionId = table.Column<int>(type: "integer", nullable: false),
                    SignalId = table.Column<string>(type: "text", nullable: false),
                    SignalType = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<float>(type: "real", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Z = table.Column<float>(type: "real", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calibrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Calibrations_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionsSignalsData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PositionId = table.Column<int>(type: "integer", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SignalId = table.Column<string>(type: "text", nullable: false),
                    SignalType = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<float>(type: "real", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Z = table.Column<float>(type: "real", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionsSignalsData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionsSignalsData_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLocalizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CalculatedPositionId = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLocalizations_Positions_CalculatedPositionId",
                        column: x => x.CalculatedPositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationMeasurements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserLocalizationId = table.Column<int>(type: "integer", nullable: false),
                    SignalId = table.Column<string>(type: "text", nullable: false),
                    SignalType = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<float>(type: "real", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Z = table.Column<float>(type: "real", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationMeasurements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalizationMeasurements_UserLocalizations_UserLocalization~",
                        column: x => x.UserLocalizationId,
                        principalTable: "UserLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calibrations_PositionId",
                table: "Calibrations",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizationMeasurements_UserLocalizationId",
                table: "LocalizationMeasurements",
                column: "UserLocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_ZoneId",
                table: "Positions",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionsSignalsData_PositionId",
                table: "PositionsSignalsData",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocalizations_CalculatedPositionId",
                table: "UserLocalizations",
                column: "CalculatedPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Zones_LocaleId",
                table: "Zones",
                column: "LocaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calibrations");

            migrationBuilder.DropTable(
                name: "LocalizationMeasurements");

            migrationBuilder.DropTable(
                name: "PositionsSignalsData");

            migrationBuilder.DropTable(
                name: "UserLocalizations");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Locales");
        }
    }
}
