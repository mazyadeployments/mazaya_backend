using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdatedCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OfferDocument_DocumentId_OfferId",
                table: "OfferDocument");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "Land",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameArabic",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEnglish",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficialEmail",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Land",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "NameArabic",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "NameEnglish",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "OfficialEmail",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfferDocument_DocumentId_OfferId",
                table: "OfferDocument",
                columns: new[] { "DocumentId", "OfferId" });
        }
    }
}
