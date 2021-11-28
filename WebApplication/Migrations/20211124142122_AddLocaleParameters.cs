using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApplication.Migrations
{
    public partial class AddLocaleParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocaleParameters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocaleId = table.Column<int>(type: "integer", nullable: true),
                    MeanError = table.Column<double>(type: "double precision", nullable: false),
                    Neighbours = table.Column<int>(type: "integer", nullable: false),
                    UnmatchedSignalsWeight = table.Column<double>(type: "double precision", nullable: false),
                    BleWeight = table.Column<double>(type: "double precision", nullable: false),
                    WifiWeight = table.Column<double>(type: "double precision", nullable: false),
                    MagnetometerWeight = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaleParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocaleParameters_Locales_LocaleId",
                        column: x => x.LocaleId,
                        principalTable: "Locales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocaleParameters_LocaleId",
                table: "LocaleParameters",
                column: "LocaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocaleParameters");
        }
    }
}
