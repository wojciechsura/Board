using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Data.SQLite.Migrations
{
    public partial class AddedLimitShownItemsToColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LimitShownItems",
                table: "Columns",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitShownItems",
                table: "Columns");
        }
    }
}
