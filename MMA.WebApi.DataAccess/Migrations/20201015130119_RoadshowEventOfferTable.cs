using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowEventOfferTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowOffer_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.DropColumn(
                name: "RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RoadshowOffer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoadshowEventOffer",
                columns: table => new
                {
                    RoadshowEventId = table.Column<int>(nullable: false),
                    RoadshowOfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowEventOffer", x => new { x.RoadshowOfferId, x.RoadshowEventId });
                    table.ForeignKey(
                        name: "FK_RoadshowEventOffer_RoadshowEvent_RoadshowEventId",
                        column: x => x.RoadshowEventId,
                        principalTable: "RoadshowEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowEventOffer_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEventOffer_RoadshowEventId",
                table: "RoadshowEventOffer",
                column: "RoadshowEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoadshowEventOffer");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RoadshowOffer");

            migrationBuilder.AddColumn<int>(
                name: "RoadshowEventId",
                table: "RoadshowOffer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId",
                principalTable: "RoadshowEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
