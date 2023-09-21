using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class removeTempImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TempImages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempImages",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                        UploadTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempImages", x => x.Id);
                }
            );
        }
    }
}
