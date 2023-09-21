using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class DeletedCompanyIdFromRoadshowOffer2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowOffer_CompanyId",
                table: "RoadshowOffer");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "RoadshowOffer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "RoadshowOffer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_CompanyId",
                table: "RoadshowOffer",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
