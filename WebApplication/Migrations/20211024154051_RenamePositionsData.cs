using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApplication.Migrations
{
    public partial class RenamePositionsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionsData");

            migrationBuilder.CreateTable(
                name: "PositionsSignalsData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PositionId = table.Column<int>(type: "integer", nullable: false),
                    Samples = table.Column<int>(type: "integer", nullable: false),
                    StandardDeviation = table.Column<float>(type: "real", nullable: false),
                    Max = table.Column<float>(type: "real", nullable: false),
                    Min = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationX = table.Column<float>(type: "real", nullable: false),
                    MaxX = table.Column<float>(type: "real", nullable: false),
                    MinX = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationY = table.Column<float>(type: "real", nullable: false),
                    MaxY = table.Column<float>(type: "real", nullable: false),
                    MinY = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationZ = table.Column<float>(type: "real", nullable: false),
                    MaxZ = table.Column<float>(type: "real", nullable: false),
                    MinZ = table.Column<float>(type: "real", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_PositionsSignalsData_PositionId",
                table: "PositionsSignalsData",
                column: "PositionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionsSignalsData");

            migrationBuilder.CreateTable(
                name: "PositionsData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Max = table.Column<float>(type: "real", nullable: false),
                    MaxX = table.Column<float>(type: "real", nullable: false),
                    MaxY = table.Column<float>(type: "real", nullable: false),
                    MaxZ = table.Column<float>(type: "real", nullable: false),
                    Min = table.Column<float>(type: "real", nullable: false),
                    MinX = table.Column<float>(type: "real", nullable: false),
                    MinY = table.Column<float>(type: "real", nullable: false),
                    MinZ = table.Column<float>(type: "real", nullable: false),
                    PositionId = table.Column<int>(type: "integer", nullable: false),
                    Samples = table.Column<int>(type: "integer", nullable: false),
                    SignalId = table.Column<string>(type: "text", nullable: false),
                    SignalType = table.Column<int>(type: "integer", nullable: false),
                    StandardDeviation = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationX = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationY = table.Column<float>(type: "real", nullable: false),
                    StandardDeviationZ = table.Column<float>(type: "real", nullable: false),
                    Strength = table.Column<float>(type: "real", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Z = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionsData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionsData_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionsData_PositionId",
                table: "PositionsData",
                column: "PositionId");
        }
    }
}
