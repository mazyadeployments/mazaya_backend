using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoofAdult",
                table: "MazayaSubcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "optiontype",
                table: "MazayaSubcategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "totalcount",
                table: "MazayaSubcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "vat",
                table: "MazayaSubcategories",
                type: "decimal(28,12)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoofAdult",
                table: "MazayaSubcategories");

            migrationBuilder.DropColumn(
                name: "optiontype",
                table: "MazayaSubcategories");

            migrationBuilder.DropColumn(
                name: "totalcount",
                table: "MazayaSubcategories");

            migrationBuilder.DropColumn(
                name: "vat",
                table: "MazayaSubcategories");
        }
    }
}
