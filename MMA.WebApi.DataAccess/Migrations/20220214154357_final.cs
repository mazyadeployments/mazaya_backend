using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class final : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferReports_Offer_OfferId",
                table: "OfferReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfferReports",
                table: "OfferReports");

            migrationBuilder.RenameTable(
                name: "OfferReports",
                newName: "OfferReport");

            migrationBuilder.RenameIndex(
                name: "IX_OfferReports_OfferId",
                table: "OfferReport",
                newName: "IX_OfferReport_OfferId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfferReport",
                table: "OfferReport",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferReport_Offer_OfferId",
                table: "OfferReport",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferReport_Offer_OfferId",
                table: "OfferReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OfferReport",
                table: "OfferReport");

            migrationBuilder.RenameTable(
                name: "OfferReport",
                newName: "OfferReports");

            migrationBuilder.RenameIndex(
                name: "IX_OfferReport_OfferId",
                table: "OfferReports",
                newName: "IX_OfferReports_OfferId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OfferReports",
                table: "OfferReports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferReports_Offer_OfferId",
                table: "OfferReports",
                column: "OfferId",
                principalTable: "Offer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
