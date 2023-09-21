using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class MailStorageDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MailStorageDocument",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        MailStorageId = table.Column<int>(nullable: false),
                        DocumentId = table.Column<Guid>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailStorageDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MailStorageDocument_MailStorage_MailStorageId",
                        column: x => x.MailStorageId,
                        principalTable: "MailStorage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_MailStorageDocument_MailStorageId",
                table: "MailStorageDocument",
                column: "MailStorageId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MailStorageDocument");
        }
    }
}
