using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowCommentsAddedFKToRoadshow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoadshowComment_RoadshowId",
                table: "RoadshowComment",
                column: "RoadshowId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowComment_Roadshow_RoadshowId",
                table: "RoadshowComment",
                column: "RoadshowId",
                principalTable: "Roadshow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowComment_Roadshow_RoadshowId",
                table: "RoadshowComment");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowComment_RoadshowId",
                table: "RoadshowComment");
        }
    }
}
