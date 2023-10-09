using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mazayacategorydetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    relation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MazayasubCategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mazayacategorydetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mazayacategorydetails_MazayaSubcategories_MazayasubCategoryId",
                        column: x => x.MazayasubCategoryId,
                        principalTable: "MazayaSubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mazayacategoryDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mazayaCategoryId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    OriginalImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    X1 = table.Column<double>(type: "float", nullable: false),
                    Y1 = table.Column<double>(type: "float", nullable: false),
                    X2 = table.Column<double>(type: "float", nullable: false),
                    Y2 = table.Column<double>(type: "float", nullable: false),
                    cropX1 = table.Column<double>(type: "float", nullable: false),
                    cropY1 = table.Column<double>(type: "float", nullable: false),
                    cropX2 = table.Column<double>(type: "float", nullable: false),
                    cropY2 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mazayacategoryDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mazayacategoryDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mazayacategoryDocument_MazayaCategories_mazayaCategoryId",
                        column: x => x.mazayaCategoryId,
                        principalTable: "MazayaCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mazayacategorydetails_MazayasubCategoryId",
                table: "Mazayacategorydetails",
                column: "MazayasubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_mazayacategoryDocument_DocumentId",
                table: "mazayacategoryDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_mazayacategoryDocument_mazayaCategoryId",
                table: "mazayacategoryDocument",
                column: "mazayaCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mazayacategorydetails");

            migrationBuilder.DropTable(
                name: "mazayacategoryDocument");
        }
    }
}
