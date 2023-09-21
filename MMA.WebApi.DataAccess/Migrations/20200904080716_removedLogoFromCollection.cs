using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class removedLogoFromCollection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SponsorLogoImageUrl",
                table: "Collection");

            migrationBuilder.DropColumn(
                name: "SponsorLogoImageUrlCrop",
                table: "Collection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SponsorLogoImageUrl",
                table: "Collection",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SponsorLogoImageUrlCrop",
                table: "Collection",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
