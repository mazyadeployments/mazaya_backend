using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedCoordinatesForRoadshowDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OriginalImageId",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<double>(
                name: "X1",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "X2",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y1",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Y2",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropX1",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropX2",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropY1",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "cropY2",
                table: "RoadshowDocument",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalImageId",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "X1",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "X2",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "Y1",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "Y2",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "cropX1",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "cropX2",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "cropY1",
                table: "RoadshowDocument");

            migrationBuilder.DropColumn(
                name: "cropY2",
                table: "RoadshowDocument");
        }
    }
}
