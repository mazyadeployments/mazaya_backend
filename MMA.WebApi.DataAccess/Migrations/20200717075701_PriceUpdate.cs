using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class PriceUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Offer",
                newName: "PriceTo");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "Offer",
                newName: "PriceFrom");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountFrom",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountTo",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceCustom",
                table: "Offer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountFrom",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "DiscountTo",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "PriceCustom",
                table: "Offer");

            migrationBuilder.RenameColumn(
                name: "PriceTo",
                table: "Offer",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "PriceFrom",
                table: "Offer",
                newName: "DiscountPercentage");
        }
    }
}
