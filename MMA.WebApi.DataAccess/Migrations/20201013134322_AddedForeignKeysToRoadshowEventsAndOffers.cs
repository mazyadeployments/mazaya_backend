using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedForeignKeysToRoadshowEventsAndOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowEventId",
                table: "RoadshowOffer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowInviteId",
                table: "RoadshowEvent",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowEvent",
                column: "RoadshowInviteId",
                principalTable: "RoadshowInvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId",
                principalTable: "RoadshowEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowEventId",
                table: "RoadshowOffer",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowInviteId",
                table: "RoadshowEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                table: "RoadshowEvent",
                column: "RoadshowInviteId",
                principalTable: "RoadshowInvite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId",
                principalTable: "RoadshowEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
