using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RemoveAgreementAsEntityFromEntireSolution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountInfo_OfferAgreement_OfferAgreementId",
                table: "DiscountInfo"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_MailStorage_OfferAgreement_AgreementId",
                table: "MailStorage"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_OfferAgreement_OfferAgreementId",
                table: "Offer"
            );

            migrationBuilder.DropTable(name: "OfferAgreementComment");

            migrationBuilder.DropTable(name: "OfferAgreementDocument");

            migrationBuilder.DropTable(name: "OfferAgreement");

            migrationBuilder.DropIndex(name: "IX_Offer_OfferAgreementId", table: "Offer");

            migrationBuilder.DropIndex(name: "IX_MailStorage_AgreementId", table: "MailStorage");

            migrationBuilder.DropIndex(
                name: "IX_DiscountInfo_OfferAgreementId",
                table: "DiscountInfo"
            );

            migrationBuilder.DropColumn(name: "AgreementId", table: "UserNotification");

            migrationBuilder.DropColumn(name: "OfferAgreementId", table: "Offer");

            migrationBuilder.DropColumn(name: "AgreementId", table: "MailStorage");

            migrationBuilder.DropColumn(name: "OfferAgreementId", table: "DiscountInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgreementId",
                table: "UserNotification",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "OfferAgreementId",
                table: "Offer",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "AgreementId",
                table: "MailStorage",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "OfferAgreementId",
                table: "DiscountInfo",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "OfferAgreement",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        CompanyId = table.Column<int>(type: "int", nullable: true),
                        CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                        ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                        Manager = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        OfferEffectiveDate = table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),
                        ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        ReviewedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                        Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        TermsAndCondition = table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: true
                        ),
                        TermsAndConditionChecked = table.Column<bool>(type: "bit", nullable: false),
                        Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                        UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreement_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "OfferAgreementComment",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        CreatedBy = table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: true
                        ),
                        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                        OfferAgreementId = table.Column<int>(type: "int", nullable: false),
                        Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreementComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreementComment_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "OfferAgreementDocument",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        CreatedBy = table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: true
                        ),
                        CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                        DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        OfferAgreementId = table.Column<int>(type: "int", nullable: false),
                        Type = table.Column<int>(type: "int", nullable: false),
                        UpdatedBy = table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: true
                        ),
                        UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferAgreementDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferAgreementDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_OfferAgreementDocument_OfferAgreement_OfferAgreementId",
                        column: x => x.OfferAgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Offer_OfferAgreementId",
                table: "Offer",
                column: "OfferAgreementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_AgreementId",
                table: "MailStorage",
                column: "AgreementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_DiscountInfo_OfferAgreementId",
                table: "DiscountInfo",
                column: "OfferAgreementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreement_CompanyId",
                table: "OfferAgreement",
                column: "CompanyId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementComment_OfferAgreementId",
                table: "OfferAgreementComment",
                column: "OfferAgreementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementDocument_DocumentId",
                table: "OfferAgreementDocument",
                column: "DocumentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfferAgreementDocument_OfferAgreementId",
                table: "OfferAgreementDocument",
                column: "OfferAgreementId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountInfo_OfferAgreement_OfferAgreementId",
                table: "DiscountInfo",
                column: "OfferAgreementId",
                principalTable: "OfferAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_MailStorage_OfferAgreement_AgreementId",
                table: "MailStorage",
                column: "AgreementId",
                principalTable: "OfferAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_OfferAgreement_OfferAgreementId",
                table: "Offer",
                column: "OfferAgreementId",
                principalTable: "OfferAgreement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
