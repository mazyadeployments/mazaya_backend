using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferBannerCheckbox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AnnouncementActive",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BannerActive",
                table: "Offer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnouncementActive",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "BannerActive",
                table: "Offer");
        }
    }
}
