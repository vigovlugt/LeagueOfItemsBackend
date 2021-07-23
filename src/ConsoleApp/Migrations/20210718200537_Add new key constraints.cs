using Microsoft.EntityFrameworkCore.Migrations;

namespace LeagueOfItems.ConsoleApp.Migrations
{
    public partial class Addnewkeyconstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RuneData",
                table: "RuneData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemData",
                table: "ItemData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChampionData",
                table: "ChampionData");

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "RuneData",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "ItemData",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "ChampionData",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuneData",
                table: "RuneData",
                columns: new[] { "RuneId", "ChampionId", "Rank", "Tier", "Region", "Role", "Patch" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemData",
                table: "ItemData",
                columns: new[] { "ItemId", "ChampionId", "Rank", "Order", "Region", "Role", "Patch" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChampionData",
                table: "ChampionData",
                columns: new[] { "ChampionId", "Rank", "Region", "Role", "Patch" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RuneData",
                table: "RuneData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemData",
                table: "ItemData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChampionData",
                table: "ChampionData");

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "RuneData",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "ItemData",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Patch",
                table: "ChampionData",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuneData",
                table: "RuneData",
                columns: new[] { "RuneId", "ChampionId", "Rank", "Tier", "Region", "Role" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemData",
                table: "ItemData",
                columns: new[] { "ItemId", "ChampionId", "Rank", "Order", "Region", "Role" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChampionData",
                table: "ChampionData",
                columns: new[] { "ChampionId", "Rank", "Region", "Role" });
        }
    }
}
