using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class bannerFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Banner_BannerId",
                table: "Offer");

            migrationBuilder.DropTable(
                name: "Banner");

            migrationBuilder.DropIndex(
                name: "IX_Offer_BannerId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "BannerId",
                table: "Offer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BannerId",
                table: "Offer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banner", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_BannerId",
                table: "Offer",
                column: "BannerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Banner_BannerId",
                table: "Offer",
                column: "BannerId",
                principalTable: "Banner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
