using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Roadshow_Removed_Location_Property : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roadshow_DefaultLocation_LocationId",
                table: "Roadshow");

            migrationBuilder.DropIndex(
                name: "IX_Roadshow_LocationId",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Roadshow");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Roadshow",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roadshow_LocationId",
                table: "Roadshow",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roadshow_DefaultLocation_LocationId",
                table: "Roadshow",
                column: "LocationId",
                principalTable: "DefaultLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
