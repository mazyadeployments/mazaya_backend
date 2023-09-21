using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedForeignKeyToInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                table: "RoadshowInvite");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowId",
                table: "RoadshowInvite",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                table: "RoadshowInvite",
                column: "RoadshowId",
                principalTable: "Roadshow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                table: "RoadshowInvite");

            migrationBuilder.AlterColumn<int>(
                name: "RoadshowId",
                table: "RoadshowInvite",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                table: "RoadshowInvite",
                column: "RoadshowId",
                principalTable: "Roadshow",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
