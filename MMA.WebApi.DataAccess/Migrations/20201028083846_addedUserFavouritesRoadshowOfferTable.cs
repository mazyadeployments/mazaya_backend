using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class addedUserFavouritesRoadshowOfferTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFavouritesRoadshowOffer",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavouritesRoadshowOffer", x => new { x.RoadshowOfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_UserFavouritesRoadshowOffer_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavouritesRoadshowOffer_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFavouritesRoadshowOffer_ApplicationUserId",
                table: "UserFavouritesRoadshowOffer",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFavouritesRoadshowOffer");
        }
    }
}
