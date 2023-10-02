using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "MazayaSubcategories",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MazayaPackageSubscriptions",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "MazayaPackageSubscriptions",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "MazayaCategories",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "MazayaCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MazayaPackageSubscriptions_ApplicationUserId",
                table: "MazayaPackageSubscriptions",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MazayaPackageSubscriptions_AspNetUsers_ApplicationUserId",
                table: "MazayaPackageSubscriptions",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MazayaPackageSubscriptions_AspNetUsers_ApplicationUserId",
                table: "MazayaPackageSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_MazayaPackageSubscriptions_ApplicationUserId",
                table: "MazayaPackageSubscriptions");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "MazayaSubcategories");

            migrationBuilder.DropColumn(
                name: "date",
                table: "MazayaPackageSubscriptions");

            migrationBuilder.DropColumn(
                name: "image",
                table: "MazayaCategories");

            migrationBuilder.DropColumn(
                name: "status",
                table: "MazayaCategories");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "MazayaPackageSubscriptions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
