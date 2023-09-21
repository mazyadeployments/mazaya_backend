using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferFKs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("delete from offer");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CompanyId",
                table: "Offer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_OfferAgreementId",
                table: "Offer",
                column: "OfferAgreementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Company_CompanyId",
                table: "Offer",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_OfferAgreement_OfferAgreementId",
                table: "Offer",
                column: "OfferAgreementId",
                principalTable: "OfferAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Company_CompanyId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_OfferAgreement_OfferAgreementId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_CompanyId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_OfferAgreementId",
                table: "Offer");
        }
    }
}
