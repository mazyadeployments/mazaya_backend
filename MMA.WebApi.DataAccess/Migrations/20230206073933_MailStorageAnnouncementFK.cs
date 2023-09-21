using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class MailStorageAnnouncementFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnnouncementId",
                table: "MailStorage",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MailStorage_AnnouncementId",
                table: "MailStorage",
                column: "AnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_MailStorage_Announcement_AnnouncementId",
                table: "MailStorage",
                column: "AnnouncementId",
                principalTable: "Announcement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MailStorage_Announcement_AnnouncementId",
                table: "MailStorage");

            migrationBuilder.DropIndex(
                name: "IX_MailStorage_AnnouncementId",
                table: "MailStorage");

            migrationBuilder.DropColumn(
                name: "AnnouncementId",
                table: "MailStorage");
        }
    }
}
