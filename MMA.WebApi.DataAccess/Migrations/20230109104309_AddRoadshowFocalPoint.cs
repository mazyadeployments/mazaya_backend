using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddRoadshowFocalPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    {
                        "42ff92cd-dc4d-4943-961a-905a3c8bd401",
                        "RoadshowFocalPoint",
                        "Roadshow Focal Point",
                        "931c56d0-bb33-4158-8aae-5443ad3f0ab1"
                    }
                }
            );

            /* migrationBuilder.InsertData(
                     table: "EmailTemplate",
                     columns: new[] { "Id", "Name", "Subject", "Message", "Notification", "NotificationTypeId", "Sms", "CreatedOn", "CreatedBy", "UpdatedOn", "UpdatedBy" },
                     values: new object[,]
                     {
                         { 42, "Roadshow_Confirmed_Notify_All", "Roadshow confirmed", "Roadshow has been confirmed.", "Roadshow has been confirmed.", 1, null, DateTime.UtcNow, "sysadmin", DateTime.UtcNow, "sysadmin" }
                     }
                 );*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "EmailTemplate", keyColumn: "Id", keyValue: 42);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42ff92cd-dc4d-4943-961a-905a3c8bd401"
            );
        }
    }
}
