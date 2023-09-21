using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class CategoryCollectionDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryDocument_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectionDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    OriginalImageId = table.Column<Guid>(nullable: false),
                    X1 = table.Column<double>(nullable: false),
                    Y1 = table.Column<double>(nullable: false),
                    X2 = table.Column<double>(nullable: false),
                    Y2 = table.Column<double>(nullable: false),
                    cropX1 = table.Column<double>(nullable: false),
                    cropY1 = table.Column<double>(nullable: false),
                    cropX2 = table.Column<double>(nullable: false),
                    cropY2 = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionDocument_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDocument_CategoryId",
                table: "CategoryDocument",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDocument_DocumentId",
                table: "CategoryDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionDocument_CollectionId",
                table: "CollectionDocument",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionDocument_DocumentId",
                table: "CollectionDocument",
                column: "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDocument");

            migrationBuilder.DropTable(
                name: "CollectionDocument");
        }
    }
}
