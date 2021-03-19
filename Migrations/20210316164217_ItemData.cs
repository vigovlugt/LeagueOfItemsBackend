using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.Migrations
{
    public partial class ItemData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemGold");

            migrationBuilder.CreateTable(
                name: "Champions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Blurb = table.Column<string>(type: "TEXT", nullable: true),
                    RiotId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Champions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemData",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Region = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    Matches = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemData", x => new { x.ItemId, x.ChampionId, x.Rank, x.Order, x.Region, x.Role });
                });

            migrationBuilder.CreateTable(
                name: "StarterSetData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChampionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Region = table.Column<int>(type: "INTEGER", nullable: false),
                    Rank = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    Matches = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarterSetData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StarterSetItems",
                columns: table => new
                {
                    StarterSetId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarterSetItems", x => new { x.StarterSetId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_StarterSetItems_StarterSetData_StarterSetId",
                        column: x => x.StarterSetId,
                        principalTable: "StarterSetData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Champions");

            migrationBuilder.DropTable(
                name: "ItemData");

            migrationBuilder.DropTable(
                name: "StarterSetItems");

            migrationBuilder.DropTable(
                name: "StarterSetData");

            migrationBuilder.CreateTable(
                name: "ItemGold",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Base = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Purchasable = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sell = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemGold", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemGold_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemGold_ItemId",
                table: "ItemGold",
                column: "ItemId",
                unique: true);
        }
    }
}
