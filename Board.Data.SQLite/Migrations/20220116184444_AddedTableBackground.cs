using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Data.SQLite.Migrations
{
    public partial class AddedTableBackground : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Tables",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "Tables");
        }
    }
}
