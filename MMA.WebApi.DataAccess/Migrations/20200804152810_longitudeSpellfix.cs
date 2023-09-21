using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class longitudeSpellfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Longtitude",
                table: "OfferLocation");

            migrationBuilder.DropColumn(
                name: "Longtitude",
                table: "CompanyLocation");

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "OfferLocation",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "CompanyLocation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "OfferLocation");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "CompanyLocation");

            migrationBuilder.AddColumn<string>(
                name: "Longtitude",
                table: "OfferLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longtitude",
                table: "CompanyLocation",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
