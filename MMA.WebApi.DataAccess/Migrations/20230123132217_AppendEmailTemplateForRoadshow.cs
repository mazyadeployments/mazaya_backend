using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AppendEmailTemplateForRoadshow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* migrationBuilder.Sql("update EmailTemplate set Subject='Roadshow confirmed.', Message='Roadshow @@custom-message@@: has been confirmed.', Notification='Roadshow @@custom-message@@: has been confirmed.' where Id=42");
             migrationBuilder.Sql("update EmailTemplate set Subject='Roadshow approved.', Message='Roadshow @@custom-message@@: has been approved.', Notification='Roadshow @@custom-message@@: has been approved.' where Id=12");
             migrationBuilder.InsertData(
                     table: "EmailTemplate",
                     columns: new[] { "Id", "Name", "Subject", "Message", "Notification", "NotificationTypeId", "Sms", "CreatedOn", "CreatedBy", "UpdatedOn", "UpdatedBy" },
                     values: new object[,]
                     {
                         { 43, "Roadshow_Submitted_Notify_Coordinator", "New Roadshow Submitted.", "Roadshow @@custom-message@@: has been submitted for your review.", "Roadshow @@custom-message@@: has been submitted for your review.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" },
                         { 44, "Roadshow_Returned_To_Supplier_Notify_SupplierAdminOrSupplier", "Roadshow returned.", "Roadshow @@custom-message@@: has been returned for your correction.", "Roadshow @@custom-message@@: has been returned for your correction.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" },
                         { 45, "Roadshow_Reject_Attendance_Notify_Coordinator", "Roadshow rejected.", "Roadshow @@custom-message@@: has been rejected.", "Roadshow @@custom-message@@: has been rejected.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" }
                     }
                 );*/
        }

        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
