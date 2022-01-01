using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Data.SQLite.Migrations
{
    public partial class AddedIsDeletedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tables",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Entries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Columns",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Columns");
        }
    }
}
