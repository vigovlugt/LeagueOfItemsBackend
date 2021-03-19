using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.Migrations
{
    public partial class RiotItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Colloq",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Depth",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plaintext",
                table: "Items",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemGold",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Base = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<int>(type: "INTEGER", nullable: false),
                    Sell = table.Column<int>(type: "INTEGER", nullable: false),
                    Purchasable = table.Column<bool>(type: "INTEGER", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemGold");

            migrationBuilder.DropColumn(
                name: "Colloq",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Depth",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Plaintext",
                table: "Items");
        }
    }
}
