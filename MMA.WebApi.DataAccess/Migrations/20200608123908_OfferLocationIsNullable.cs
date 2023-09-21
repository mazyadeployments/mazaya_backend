using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferLocationIsNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)",
                oldNullable: true);
        }
    }
}
