using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class addedTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Offer");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Offer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    IsEditable = table.Column<bool>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Tag",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsEditable", "Title", "UpdatedBy", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 527, DateTimeKind.Local).AddTicks(8479), true, "Special Offer", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(3190) },
                    { 2, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4125), true, "Best Price", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4172) },
                    { 3, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4189), true, "Exclusive Offer", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4194) },
                    { 4, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4199), true, "Top Seller", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4204) },
                    { 5, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4210), true, "Featured", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4215) },
                    { 6, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4220), true, "Trending", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4225) },
                    { 7, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4231), false, "Ending Soon", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4236) },
                    { 8, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4242), false, "Upcoming", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4246) },
                    { 9, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4252), false, "Latest", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4257) },
                    { 10, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4262), true, "Best Rates", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4267) },
                    { 11, "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4273), true, "Any Other Tag", "58624BF9-4931-451B-89D4-D8E3F1E6BA59", new DateTime(2020, 7, 2, 11, 15, 36, 531, DateTimeKind.Local).AddTicks(4278) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_TagId",
                table: "Offer",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Tag_TagId",
                table: "Offer",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Tag_TagId",
                table: "Offer");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropIndex(
                name: "IX_Offer_TagId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Offer");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Offer",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true);
        }
    }
}
