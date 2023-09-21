using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferSuggestionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfferSuggestions",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        UserId = table.Column<string>(nullable: true),
                        Text = table.Column<string>(nullable: true),
                        Status = table.Column<int>(nullable: false),
                        CreatedOn = table.Column<DateTime>(nullable: false),
                        CreatedBy = table.Column<string>(nullable: true),
                        UpdatedOn = table.Column<DateTime>(nullable: false),
                        UpdatedBy = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferSuggestions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_OfferSuggestions_UserId",
                table: "OfferSuggestions",
                column: "UserId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OfferSuggestions");
        }
    }
}
