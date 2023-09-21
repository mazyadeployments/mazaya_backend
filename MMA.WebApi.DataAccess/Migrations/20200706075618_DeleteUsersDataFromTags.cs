using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class DeleteUsersDataFromTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Tag");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Tag",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Tag",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Tag",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Tag",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 898, DateTimeKind.Local).AddTicks(7950), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6105) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6895), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6936) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6951), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6956) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6960), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6964) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6970), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6974) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6978), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6982) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6987), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6991) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6995), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(6999) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7004), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7008) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7013), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7017) });

            migrationBuilder.UpdateData(
                table: "Tag",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedBy", "CreatedOn", "UpdatedBy", "UpdatedOn" },
                values: new object[] { "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7023), "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 6, 9, 34, 31, 901, DateTimeKind.Local).AddTicks(7027) });
        }
    }
}
