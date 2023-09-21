using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedSupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Active", "ConcurrencyStamp", "CreatedBy", "CreatedOn", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoBase64", "SecurityStamp", "Title", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserName" },
                values: new object[] { "703A24CD-741A-402D-9FC0-821E7663E2F8", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "supplieradmin@offers.adnoc.ae", true, null, null, false, null, "supplieradmin@offers.adnoc.ae", "supplieradmin", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "supplieradmin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Active", "ConcurrencyStamp", "CreatedBy", "CreatedOn", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PhotoBase64", "SecurityStamp", "Title", "TwoFactorEnabled", "UpdatedBy", "UpdatedOn", "UserName" },
                values: new object[] { "65584013-5D0E-4807-AB08-7B95F1B09685", 1, true, "931c56d0-bb33-4158-8aae-5443ad3f0ab1", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "supplier@offers.adnoc.ae", true, null, null, false, null, "supplier@offers.adnoc.ae", "supplier", "AQAAAAEAACcQAAAAEHUi5c+Qr7XJ4g08EH0coQ56A02aTqlNeaTAAaNGjIeDD1nph7B+z6D2wQzbmJATAQ==", null, true, null, "00000000-0000-0000-0000-000000000000", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "supplier" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "703A24CD-741A-402D-9FC0-821E7663E2F8", "40C2E7E9-7379-4EC7-94E7-B28C19E43732" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "65584013-5D0E-4807-AB08-7B95F1B09685", "356C36C1-84F6-4559-AB93-1D1719FEA54F" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "65584013-5D0E-4807-AB08-7B95F1B09685", "356C36C1-84F6-4559-AB93-1D1719FEA54F" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "703A24CD-741A-402D-9FC0-821E7663E2F8", "40C2E7E9-7379-4EC7-94E7-B28C19E43732" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "65584013-5D0E-4807-AB08-7B95F1B09685");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "703A24CD-741A-402D-9FC0-821E7663E2F8");
        }
    }
}
