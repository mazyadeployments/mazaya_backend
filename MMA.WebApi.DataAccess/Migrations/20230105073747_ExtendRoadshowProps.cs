using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class ExtendRoadshowProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Roadshow");

            migrationBuilder.AddColumn<string>(
                name: "Activities",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructionBox",
                table: "Roadshow",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Roadshow",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roadshow_DefaultLocation_LocationId",
                table: "Roadshow");

            migrationBuilder.DropIndex(
                name: "IX_Roadshow_LocationId",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "Activities",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "InstructionBox",
                table: "Roadshow");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Roadshow");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Roadshow",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
