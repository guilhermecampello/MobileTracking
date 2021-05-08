using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class UpdateEntitiesNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZCoordinate",
                table: "Positions",
                newName: "Z");

            migrationBuilder.RenameColumn(
                name: "YCoordinate",
                table: "Positions",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "XCoordinate",
                table: "Positions",
                newName: "X");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Z",
                table: "Positions",
                newName: "ZCoordinate");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "Positions",
                newName: "YCoordinate");

            migrationBuilder.RenameColumn(
                name: "X",
                table: "Positions",
                newName: "XCoordinate");
        }
    }
}
