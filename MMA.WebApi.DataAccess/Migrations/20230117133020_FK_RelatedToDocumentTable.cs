using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class FK_RelatedToDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MailStorageDocument_DocumentId",
                table: "MailStorageDocument",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailStorageDocument_Document_DocumentId",
                table: "MailStorageDocument",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailStorageDocument_Document_DocumentId",
                table: "MailStorageDocument");

            migrationBuilder.DropIndex(
                name: "IX_MailStorageDocument_DocumentId",
                table: "MailStorageDocument");
        }
    }
}
