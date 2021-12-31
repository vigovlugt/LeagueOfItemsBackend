using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class AddBuildPathDatamodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matches",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Champions");

            migrationBuilder.CreateTable(
                name: "BuildPathData",
                columns: table => new
                {
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Item1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Item2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Item3Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Region = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Patch = table.Column<string>(type: "TEXT", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    Matches = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildPathData", x => new { x.ChampionId, x.Item1Id, x.Item2Id, x.Item3Id, x.Rank, x.Region, x.Role, x.Patch });
                    table.ForeignKey(
                        name: "FK_BuildPathData_Champions_ChampionId",
                        column: x => x.ChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPathData_Items_Item1Id",
                        column: x => x.Item1Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPathData_Items_Item2Id",
                        column: x => x.Item2Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BuildPathData_Items_Item3Id",
                        column: x => x.Item3Id,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuildPathData_Item1Id",
                table: "BuildPathData",
                column: "Item1Id");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPathData_Item2Id",
                table: "BuildPathData",
                column: "Item2Id");

            migrationBuilder.CreateIndex(
                name: "IX_BuildPathData_Item3Id",
                table: "BuildPathData",
                column: "Item3Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildPathData");

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
    }
}
