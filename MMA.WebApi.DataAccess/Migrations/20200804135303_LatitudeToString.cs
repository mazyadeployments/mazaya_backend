using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class LatitudeToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Longtitude",
                table: "OfferLocation",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "OfferLocation",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");

            migrationBuilder.AlterColumn<string>(
                name: "Longtitude",
                table: "CompanyLocation",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");

            migrationBuilder.AlterColumn<string>(
                name: "Latitude",
                table: "CompanyLocation",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(28, 12)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "OfferLocation",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "OfferLocation",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "CompanyLocation",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "CompanyLocation",
                type: "decimal(28, 12)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
