using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Announcement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementAttachments_Announcements_AnnouncementId",
                table: "AnnouncementAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSpecificBuyers_Announcements_AnnouncementId",
                table: "AnnouncementSpecificBuyers");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSpecificSuppliers_Announcements_AnnouncementId",
                table: "AnnouncementSpecificSuppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements");

            migrationBuilder.RenameTable(
                name: "Announcements",
                newName: "Announcement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcement",
                table: "Announcement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementAttachments_Announcement_AnnouncementId",
                table: "AnnouncementAttachments",
                column: "AnnouncementId",
                principalTable: "Announcement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSpecificBuyers_Announcement_AnnouncementId",
                table: "AnnouncementSpecificBuyers",
                column: "AnnouncementId",
                principalTable: "Announcement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSpecificSuppliers_Announcement_AnnouncementId",
                table: "AnnouncementSpecificSuppliers",
                column: "AnnouncementId",
                principalTable: "Announcement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementAttachments_Announcement_AnnouncementId",
                table: "AnnouncementAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSpecificBuyers_Announcement_AnnouncementId",
                table: "AnnouncementSpecificBuyers");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnouncementSpecificSuppliers_Announcement_AnnouncementId",
                table: "AnnouncementSpecificSuppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcement",
                table: "Announcement");

            migrationBuilder.RenameTable(
                name: "Announcement",
                newName: "Announcements");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementAttachments_Announcements_AnnouncementId",
                table: "AnnouncementAttachments",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSpecificBuyers_Announcements_AnnouncementId",
                table: "AnnouncementSpecificBuyers",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnouncementSpecificSuppliers_Announcements_AnnouncementId",
                table: "AnnouncementSpecificSuppliers",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
