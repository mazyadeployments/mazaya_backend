using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedOfferLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "Longtitude",
                table: "Offer");

            migrationBuilder.CreateTable(
                name: "OfferLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longtitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    OfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferLocation_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferLocation_OfferId",
                table: "OfferLocation",
                column: "OfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferLocation");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longtitude",
                table: "Offer",
                type: "decimal(28, 12)",
                nullable: true);
        }
    }
}
