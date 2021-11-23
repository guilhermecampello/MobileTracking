using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddRealXAndRealYColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserLocalizations_Positions_CalculatedPositionId",
                table: "UserLocalizations");

            migrationBuilder.DropIndex(
                name: "IX_UserLocalizations_CalculatedPositionId",
                table: "UserLocalizations");

            migrationBuilder.DropColumn(
                name: "CalculatedPositionId",
                table: "UserLocalizations");

            migrationBuilder.AddColumn<float>(
                name: "RealX",
                table: "UserLocalizations",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "RealY",
                table: "UserLocalizations",
                type: "real",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealX",
                table: "UserLocalizations");

            migrationBuilder.DropColumn(
                name: "RealY",
                table: "UserLocalizations");

            migrationBuilder.AddColumn<int>(
                name: "CalculatedPositionId",
                table: "UserLocalizations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserLocalizations_CalculatedPositionId",
                table: "UserLocalizations",
                column: "CalculatedPositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLocalizations_Positions_CalculatedPositionId",
                table: "UserLocalizations",
                column: "CalculatedPositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
