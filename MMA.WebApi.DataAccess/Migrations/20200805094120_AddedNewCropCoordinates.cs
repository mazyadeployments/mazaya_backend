using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedNewCropCoordinates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "cropX1",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropX2",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropY1",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropY2",
                table: "OfferDocument",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cropX1",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "cropX2",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "cropY1",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "cropY2",
                table: "OfferDocument");
        }
    }
}
