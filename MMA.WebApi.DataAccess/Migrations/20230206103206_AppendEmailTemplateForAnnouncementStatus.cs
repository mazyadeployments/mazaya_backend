using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AppendEmailTemplateForAnnouncementStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*  migrationBuilder.InsertData(
                      table: "EmailTemplate",
                      columns: new[] { "Id", "Name", "Subject", "Message", "Notification", "NotificationTypeId", "Sms", "CreatedOn", "CreatedBy", "UpdatedOn", "UpdatedBy" },
                      values: new object[,]
                      {
                          { 46, "Announcement_Successfully_Sent", "Announcement.", "The announcement has been successfully sent to the users.", "The announcement has been successfully sent to the users.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" },
                          { 47, "Announcement_Failed_To_Sent", "Announcement.", "The announcement has failed to send to the users, please contact Help Desk.", "The announcement has failed to send to the users, please contact Help Desk.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" },
                      }
                  );*/
        }

        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
