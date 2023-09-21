using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedNewUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B6E82FB2-CAC5-43E4-BCF4-4156AA829D78",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Active", "ConcurrencyStamp", "CreatedBy", "CreatedOn", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoBase64", "SecurityStamp", "Title", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserName" },
                values: new object[,]
                {
                    { "7649070E-B684-4F70-BBCB-8346AC796310", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1@offers.adnoc.ae", true, null, null, false, null, "user1@offers.adnoc.ae", "User 1", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1" },
                    { "5C7B9785-5755-428A-B741-35A75150870C", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2@offers.adnoc.ae", true, null, null, false, null, "user2@offers.adnoc.ae", "User 2", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2" },
                    { "6296B4B7-CC01-467C-A621-979F95E494A0", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user3@offers.adnoc.ae", true, null, null, false, null, "user3@offers.adnoc.ae", "User 3", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user3" },
                    { "76FCE130-6E57-418F-A71B-5ABC235F6AC4", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "reviewer@offers.adnoc.ae", true, null, null, false, null, "reviewer@offers.adnoc.ae", "Reviewer", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "reviewer" },
                    { "268B8B27-C510-4558-B591-9BDFBF9FF76F", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "coordinator@offers.adnoc.ae", true, null, null, false, null, "coordinator@offers.adnoc.ae", "Coordinator", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "coordinator" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[,]
                {
                    { "7649070E-B684-4F70-BBCB-8346AC796310", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" },
                    { "5C7B9785-5755-428A-B741-35A75150870C", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" },
                    { "6296B4B7-CC01-467C-A621-979F95E494A0", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" },
                    { "76FCE130-6E57-418F-A71B-5ABC235F6AC4", "92763DED-6382-4B5F-B5E2-1ABBA6551F34" },
                    { "268B8B27-C510-4558-B591-9BDFBF9FF76F", "B8C2DCD9-B661-4E50-903C-663FF8AA3C2E" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "268B8B27-C510-4558-B591-9BDFBF9FF76F", "B8C2DCD9-B661-4E50-903C-663FF8AA3C2E" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "5C7B9785-5755-428A-B741-35A75150870C", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "6296B4B7-CC01-467C-A621-979F95E494A0", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "7649070E-B684-4F70-BBCB-8346AC796310", "EAC9AE7A-761E-4910-B1BE-CC670FCFFC01" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "76FCE130-6E57-418F-A71B-5ABC235F6AC4", "92763DED-6382-4B5F-B5E2-1ABBA6551F34" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "268B8B27-C510-4558-B591-9BDFBF9FF76F");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5C7B9785-5755-428A-B741-35A75150870C");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6296B4B7-CC01-467C-A621-979F95E494A0");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7649070E-B684-4F70-BBCB-8346AC796310");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "76FCE130-6E57-418F-A71B-5ABC235F6AC4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B6E82FB2-CAC5-43E4-BCF4-4156AA829D78",
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHpPVSGYIdjBzBDUxZLKgZXf6B+gDttqVpqR0ttR76S7egA4x/DQfj+VmIeMTWPzEA==");
        }
    }
}
