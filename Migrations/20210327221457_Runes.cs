using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.Migrations
{
    public partial class Runes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StarterSetItems_StarterSetData_StarterSetId",
                table: "StarterSetItems");

            migrationBuilder.AddColumn<int>(
                name: "UggStarterSetId",
                table: "StarterSetItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RunePaths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RunePaths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Tier = table.Column<int>(type: "INTEGER", nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", nullable: true),
                    LongDescription = table.Column<string>(type: "TEXT", nullable: true),
                    RunePathId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runes_RunePaths_RunePathId",
                        column: x => x.RunePathId,
                        principalTable: "RunePaths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarterSetItems_UggStarterSetId",
                table: "StarterSetItems",
                column: "UggStarterSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Runes_RunePathId",
                table: "Runes",
                column: "RunePathId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemData_Items_ItemId",
                table: "ItemData",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StarterSetItems_StarterSetData_UggStarterSetId",
                table: "StarterSetItems",
                column: "UggStarterSetId",
                principalTable: "StarterSetData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemData_Items_ItemId",
                table: "ItemData");

            migrationBuilder.DropForeignKey(
                name: "FK_StarterSetItems_StarterSetData_UggStarterSetId",
                table: "StarterSetItems");

            migrationBuilder.DropTable(
                name: "Runes");

            migrationBuilder.DropTable(
                name: "RunePaths");

            migrationBuilder.DropIndex(
                name: "IX_StarterSetItems_UggStarterSetId",
                table: "StarterSetItems");

            migrationBuilder.DropColumn(
                name: "UggStarterSetId",
                table: "StarterSetItems");

            migrationBuilder.AddForeignKey(
                name: "FK_StarterSetItems_StarterSetData_StarterSetId",
                table: "StarterSetItems",
                column: "StarterSetId",
                principalTable: "StarterSetData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
