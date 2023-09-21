using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommentText",
                table: "OfferRating",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "OfferRating",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "OfferRating",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "OfferRating",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "OfferRating",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "OfferRating",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentText",
                table: "OfferRating");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "OfferRating");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "OfferRating");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OfferRating");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "OfferRating");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "OfferRating");
        }
    }
}
