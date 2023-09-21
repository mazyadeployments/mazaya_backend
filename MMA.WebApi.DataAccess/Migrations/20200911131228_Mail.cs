using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Mail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Notification = table.Column<string>(nullable: true),
                    NotificationTypeId = table.Column<int>(nullable: false),
                    Sms = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplateRoot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MailTemplate = table.Column<string>(nullable: true),
                    MailBodyFooter = table.Column<string>(nullable: true),
                    MailApplicationLogin = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplateRoot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailStorage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    OfferId = table.Column<int>(nullable: true),
                    AgreementId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    StatusOn = table.Column<DateTime>(nullable: false),
                    StatusNote = table.Column<string>(nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailStorage_OfferAgreement_AgreementId",
                        column: x => x.AgreementId,
                        principalTable: "OfferAgreement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailStorage_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MailStorage_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_AgreementId",
                table: "MailStorage",
                column: "AgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_OfferId",
                table: "MailStorage",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_UserId",
                table: "MailStorage",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "EmailTemplateRoot");

            migrationBuilder.DropTable(
                name: "MailStorage");
        }
    }
}
