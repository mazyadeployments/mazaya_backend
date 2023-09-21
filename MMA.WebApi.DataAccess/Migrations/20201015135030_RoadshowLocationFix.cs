﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowLocationFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationDefaultLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoadshowLocation",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationId",
                table: "RoadshowEvent",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowLocation_DefaultLocationId",
                table: "RoadshowLocation",
                column: "DefaultLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent",
                column: "RoadshowLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationId",
                table: "RoadshowEvent",
                column: "RoadshowLocationId",
                principalTable: "RoadshowLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowLocation_DefaultLocationId",
                table: "RoadshowLocation");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationDefaultLocationId",
                table: "RoadshowEvent",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationRoadshowId",
                table: "RoadshowEvent",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation",
                columns: new[] { "DefaultLocationId", "RoadshowId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent",
                columns: new[] { "RoadshowLocationDefaultLocationId", "RoadshowLocationRoadshowId" });

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent",
                columns: new[] { "RoadshowLocationDefaultLocationId", "RoadshowLocationRoadshowId" },
                principalTable: "RoadshowLocation",
                principalColumns: new[] { "DefaultLocationId", "RoadshowId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}