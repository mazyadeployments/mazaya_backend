using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Roadshow_AddedEmiratesIDProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(name: "EmiratesId", table: "Roadshow", nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roadshow_EmiratesId",
                table: "Roadshow",
                column: "EmiratesId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Roadshow_Document_EmiratesId",
                table: "Roadshow",
                column: "EmiratesId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roadshow_Document_EmiratesId",
                table: "Roadshow"
            );

            migrationBuilder.DropIndex(name: "IX_Roadshow_EmiratesId", table: "Roadshow");

            migrationBuilder.DropColumn(name: "EmiratesId", table: "Roadshow");
        }
    }
}
