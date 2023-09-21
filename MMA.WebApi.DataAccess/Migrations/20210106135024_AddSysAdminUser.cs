using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddSysAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Active", "ConcurrencyStamp", "CreatedBy", "CreatedOn", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Title", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserName", "LastDataSynchronizationOn", "ECardSequence", "UserType" },
                values: new object[,]
                {
                    { "0C7B881A-F0C8-4B55-861B-A1E600AA6E11", 1, true, "cab039e5-67fa-411a-87da-16b9a04d4c0e", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sysadmin", true, null, null, false, null, "sysadmin", "sysadmin", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sysadmin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 7/*Declares.UserType.Other*/ },
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "0C7B881A-F0C8-4B55-861B-A1E600AA6E11", "C9EED9D2-4DFD-4B9F-B424-BA80E021889E" },
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "AspNetUserRoles",
               keyColumns: new[] { "UserId", "RoleId" },
               keyValues: new object[] { "0C7B881A-F0C8-4B55-861B-A1E600AA6E11", "C9EED9D2-4DFD-4B9F-B424-BA80E021889E" });


            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0C7B881A-F0C8-4B55-861B-A1E600AA6E11");
        }
    }
}
