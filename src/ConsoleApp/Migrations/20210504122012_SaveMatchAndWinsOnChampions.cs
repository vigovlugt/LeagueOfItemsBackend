using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class SaveMatchAndWinsOnChampions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UggStarterSetItem");

            migrationBuilder.DropTable(
                name: "StarterSetData");

            migrationBuilder.AddColumn<int>(
                name: "Matches",
                table: "Champions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "Champions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matches",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Champions");

            migrationBuilder.CreateTable(
                name: "StarterSetData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Matches = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Region = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarterSetData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UggStarterSetItem",
                columns: table => new
                {
                    StarterSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UggStarterSetItem", x => new { x.StarterSetId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_UggStarterSetItem_StarterSetData_StarterSetId",
                        column: x => x.StarterSetId,
                        principalTable: "StarterSetData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
