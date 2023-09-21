using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdatedCompanyPhoneModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaxCountryCode",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxE164Number",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandCountryCode",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandE164Number",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LandNumber",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileCountryCode",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileE164Number",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaxCountryCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "FaxE164Number",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LandCountryCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LandE164Number",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LandNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "MobileCountryCode",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "MobileE164Number",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Company");
        }
    }
}
