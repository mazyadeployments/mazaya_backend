using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedDefaultLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "Vicinity",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "DefaultLocationId",
                table: "RoadshowLocation",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationDefaultLocationId",
                table: "RoadshowEvent",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationRoadshowId",
                table: "RoadshowEvent",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation",
                columns: new[] { "DefaultLocationId", "RoadshowId" });

            migrationBuilder.CreateTable(
                name: "DefaultLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultLocation", x => x.Id);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowLocation_DefaultLocation_DefaultLocationId",
                table: "RoadshowLocation",
                column: "DefaultLocationId",
                principalTable: "DefaultLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowLocation_DefaultLocation_DefaultLocationId",
                table: "RoadshowLocation");

            migrationBuilder.DropTable(
                name: "DefaultLocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowEvent_RoadshowLocationDefaultLocationId_RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "DefaultLocationId",
                table: "RoadshowLocation");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationDefaultLocationId",
                table: "RoadshowEvent");

            migrationBuilder.DropColumn(
                name: "RoadshowLocationRoadshowId",
                table: "RoadshowEvent");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoadshowLocation",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "RoadshowLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "RoadshowLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "RoadshowLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "RoadshowLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vicinity",
                table: "RoadshowLocation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoadshowLocationId",
                table: "RoadshowEvent",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoadshowLocation",
                table: "RoadshowLocation",
                column: "Id");

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
    }
}
