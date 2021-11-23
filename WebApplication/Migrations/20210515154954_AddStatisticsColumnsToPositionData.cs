using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddStatisticsColumnsToPositionSignalData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Max",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxX",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxY",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxZ",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Min",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinX",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinY",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinZ",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Samples",
                table: "PositionsSignalsData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviation",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationX",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationY",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationZ",
                table: "PositionsSignalsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MaxX",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MaxY",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MaxZ",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MinX",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MinY",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "MinZ",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "Samples",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviation",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationX",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationY",
                table: "PositionsSignalsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationZ",
                table: "PositionsSignalsData");
        }
    }
}
