using LeagueOfItems.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeagueOfItems.ConsoleApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260502153600_AddItemDataSlot")]
    public partial class AddItemDataSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "ItemData",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slot",
                table: "ItemData");
        }
    }
}
