using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class Bans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bans",
                table: "ChampionData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bans",
                table: "ChampionData");
        }
    }
}
