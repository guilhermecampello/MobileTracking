using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddStatisticsColumnsToPositionData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Max",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxX",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxY",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MaxZ",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Min",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinX",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinY",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinZ",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "Samples",
                table: "PositionsData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviation",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationX",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationY",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StandardDeviationZ",
                table: "PositionsData",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MaxX",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MaxY",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MaxZ",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MinX",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MinY",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "MinZ",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "Samples",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviation",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationX",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationY",
                table: "PositionsData");

            migrationBuilder.DropColumn(
                name: "StandardDeviationZ",
                table: "PositionsData");
        }
    }
}
