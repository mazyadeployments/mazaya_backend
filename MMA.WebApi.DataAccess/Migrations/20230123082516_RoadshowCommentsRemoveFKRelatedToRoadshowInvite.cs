using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowCommentsRemoveFKRelatedToRoadshowInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowInviteId",
                table: "RoadshowComment",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment",
                column: "RoadshowInviteId",
                principalTable: "RoadshowInvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowInviteId",
                table: "RoadshowComment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowComment",
                column: "RoadshowInviteId",
                principalTable: "RoadshowInvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
