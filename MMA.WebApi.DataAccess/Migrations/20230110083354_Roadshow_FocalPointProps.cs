using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Roadshow_FocalPointProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E164Number",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FocalPointEmail",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FocalPointName",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FocalPointSurname",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternationalNumber",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Roadshow",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "E164Number",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "FocalPointEmail",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "FocalPointName",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "FocalPointSurname",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "InternationalNumber",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Roadshow");
        }
    }
}
