using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class CompanyFacebook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SocialMedia",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "Facebook",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Twitter",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facebook",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Twitter",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "SocialMedia",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
