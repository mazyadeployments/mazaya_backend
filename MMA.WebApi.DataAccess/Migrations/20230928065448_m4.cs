using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "MazayaCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "facilities",
                table: "MazayaCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "MazayaCategories");

            migrationBuilder.DropColumn(
                name: "facilities",
                table: "MazayaCategories");
        }
    }
}
