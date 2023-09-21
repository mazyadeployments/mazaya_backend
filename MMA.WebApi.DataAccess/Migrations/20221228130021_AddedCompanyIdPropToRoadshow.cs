using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedCompanyIdPropToRoadshow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roadshow_CompanyId",
                table: "Roadshow",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roadshow_Company_CompanyId",
                table: "Roadshow",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roadshow_Company_CompanyId",
                table: "Roadshow");

            migrationBuilder.DropIndex(
                name: "IX_Roadshow_CompanyId",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Roadshow");
        }
    }
}
