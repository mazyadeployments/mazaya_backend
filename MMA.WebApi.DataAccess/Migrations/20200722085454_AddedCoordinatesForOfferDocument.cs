using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedCoordinatesForOfferDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "X1",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "X2",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y1",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y2",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X1",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "X2",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "Y1",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "Y2",
                table: "OfferDocument");
        }
    }
}
