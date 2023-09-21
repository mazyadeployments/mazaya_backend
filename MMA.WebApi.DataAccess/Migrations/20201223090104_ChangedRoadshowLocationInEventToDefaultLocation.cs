using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class ChangedRoadshowLocationInEventToDefaultLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_DefaultLocation_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "DefaultLocationId",
                table: "RoadshowEvent",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_DefaultLocationId",
                table: "RoadshowEvent",
                column: "DefaultLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_DefaultLocation_DefaultLocationId",
                table: "RoadshowEvent",
                column: "DefaultLocationId",
                principalTable: "DefaultLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_DefaultLocation_DefaultLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_DefaultLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "DefaultLocationId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationId",
                table: "RoadshowEvent",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent",
                column: "RoadshowLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_DefaultLocation_RoadshowLocationId",
                table: "RoadshowEvent",
                column: "RoadshowLocationId",
                principalTable: "DefaultLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
