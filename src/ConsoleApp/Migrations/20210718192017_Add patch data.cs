using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class Addpatchdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Patch",
                table: "RuneData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patch",
                table: "ItemData",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patch",
                table: "ChampionData",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Patch",
                table: "RuneData");

            migrationBuilder.DropColumn(
                name: "Patch",
                table: "ItemData");

            migrationBuilder.DropColumn(
                name: "Patch",
                table: "ChampionData");
        }
    }
}
