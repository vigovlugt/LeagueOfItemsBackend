using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class AddLoreToChampions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Lore",
                table: "Champions",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lore",
                table: "Champions");
        }
    }
}
