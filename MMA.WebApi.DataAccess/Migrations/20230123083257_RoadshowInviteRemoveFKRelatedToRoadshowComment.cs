using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowInviteRemoveFKRelatedToRoadshowComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowComment_RoadshowInviteId",
                table: "RoadshowComment");

            migrationBuilder.DropColumn(
                name: "RoadshowInviteId",
                table: "RoadshowComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoadshowInviteId",
                table: "RoadshowComment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowComment_RoadshowInviteId",
                table: "RoadshowComment",
                column: "RoadshowInviteId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment",
                column: "RoadshowInviteId",
                principalTable: "RoadshowInvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
