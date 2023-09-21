using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class companyDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Company");

            migrationBuilder.AddColumn<int>(
                name: "LogoId",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferAgreementId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_LogoId",
                table: "Company",
                column: "LogoId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocument_DocumentId",
                table: "CompanyDocument",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_CompanyDocument_LogoId",
                table: "Company",
                column: "LogoId",
                principalTable: "CompanyDocument",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_CompanyDocument_LogoId",
                table: "Company");

            migrationBuilder.DropTable(
                name: "CompanyDocument");

            migrationBuilder.DropIndex(
                name: "IX_Company_LogoId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LogoId",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
