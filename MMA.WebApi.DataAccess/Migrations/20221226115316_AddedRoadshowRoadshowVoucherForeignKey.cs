using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedRoadshowRoadshowVoucherForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoadshowId",
                table: "RoadshowVoucher",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowId",
                table: "RoadshowVoucher",
                column: "RoadshowId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowVoucher_Roadshow_RoadshowId",
                table: "RoadshowVoucher",
                column: "RoadshowId",
                principalTable: "Roadshow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowVoucher_Roadshow_RoadshowId",
                table: "RoadshowVoucher");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowVoucher_RoadshowId",
                table: "RoadshowVoucher");

            migrationBuilder.DropColumn(
                name: "RoadshowId",
                table: "RoadshowVoucher");
        }
    }
}
