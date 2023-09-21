using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UserNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotification_NotificationType_NotificationTypeId",
                table: "UserNotification");

            migrationBuilder.DropIndex(
                name: "IX_UserNotification_NotificationTypeId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "AgendaItemId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "UserNotification");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationTypeId",
                table: "UserNotification",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AgreementId",
                table: "UserNotification",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferId",
                table: "UserNotification",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserNotification",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreementId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "UserNotification");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "UserNotification");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationTypeId",
                table: "UserNotification",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActionId",
                table: "UserNotification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgendaItemId",
                table: "UserNotification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "UserNotification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "UserNotification",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotification_NotificationTypeId",
                table: "UserNotification",
                column: "NotificationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotification_NotificationType_NotificationTypeId",
                table: "UserNotification",
                column: "NotificationTypeId",
                principalTable: "NotificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
