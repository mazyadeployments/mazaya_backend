using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddOffersAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyNationality",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstablishDate",
                table: "Company",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "Company",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDforADCCI",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegalForm",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNo",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POBox",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialMedia",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OfferAgreement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    TermsAndCondition = table.Column<string>(nullable: true),
                    TermsAndConditionChecked = table.Column<bool>(nullable: false),
                    OfferEffectiveDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    ReviewedOn = table.Column<DateTime>(nullable: false),
                    ReviewedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreement_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    OfferAgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service = table.Column<string>(nullable: true),
                    DiscountOrPrice = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    OfferAgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountInfo_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferAgreementDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferAgreementId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreementDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreementDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferAgreementDocument_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferAgreementLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    OfferAgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreementLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreementLocation_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partner",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    OfferAgreementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partner_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activity_OfferAgreementId",
                table: "Activity",
                column: "OfferAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountInfo_OfferAgreementId",
                table: "DiscountInfo",
                column: "OfferAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreement_CompanyId",
                table: "OfferAgreement",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementDocument_DocumentId",
                table: "OfferAgreementDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementDocument_OfferAgreementId",
                table: "OfferAgreementDocument",
                column: "OfferAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementLocation_OfferAgreementId",
                table: "OfferAgreementLocation",
                column: "OfferAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_Partner_OfferAgreementId",
                table: "Partner",
                column: "OfferAgreementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "DiscountInfo");

            migrationBuilder.DropTable(
                name: "OfferAgreementDocument");

            migrationBuilder.DropTable(
                name: "OfferAgreementLocation");

            migrationBuilder.DropTable(
                name: "Partner");

            migrationBuilder.DropTable(
                name: "OfferAgreement");

            migrationBuilder.DropColumn(
                name: "CompanyNationality",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "EstablishDate",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "IDforADCCI",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LegalForm",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LicenseNo",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "POBox",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "SocialMedia",
                table: "Company");
        }
    }
}
