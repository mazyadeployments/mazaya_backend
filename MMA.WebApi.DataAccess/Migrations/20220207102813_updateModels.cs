using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class updateModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Tag",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "E164Number",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternationalNumber",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Offer",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MembershipType",
                table: "Offer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "Offer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialAnnouncement",
                table: "Offer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClickCount",
                table: "LogOfferClick",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "E164Number",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "InternationalNumber",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "MembershipType",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "SpecialAnnouncement",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "ClickCount",
                table: "LogOfferClick");
        }
    }
}
