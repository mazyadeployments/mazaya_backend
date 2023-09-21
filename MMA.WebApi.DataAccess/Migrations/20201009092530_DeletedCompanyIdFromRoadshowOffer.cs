using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class DeletedCompanyIdFromRoadshowOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "RoadshowOffer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "RoadshowOffer",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_Company_CompanyId",
                table: "RoadshowOffer",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
