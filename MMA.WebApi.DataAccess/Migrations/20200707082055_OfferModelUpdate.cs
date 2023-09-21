using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class OfferModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Category_CategoryId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Collection_CollectionId",
                table: "Offer");

            migrationBuilder.DropForeignKey(
                name: "FK_Offer_Tag_TagId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_CategoryId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_CollectionId",
                table: "Offer");

            migrationBuilder.DropIndex(
                name: "IX_Offer_TagId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "CollectionId",
                table: "Offer");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Offer");

            migrationBuilder.CreateTable(
                name: "OfferCategory",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferCategory", x => new { x.OfferId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_OfferCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferCategory_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferCollection",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    CollectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferCollection", x => new { x.OfferId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_OfferCollection_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferCollection_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfferTag",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferTag", x => new { x.OfferId, x.TagId });
                    table.ForeignKey(
                        name: "FK_OfferTag_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferCategory_CategoryId",
                table: "OfferCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferCollection_CollectionId",
                table: "OfferCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferTag_TagId",
                table: "OfferTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferCategory");

            migrationBuilder.DropTable(
                name: "OfferCollection");

            migrationBuilder.DropTable(
                name: "OfferTag");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Offer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollectionId",
                table: "Offer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Offer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CategoryId",
                table: "Offer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CollectionId",
                table: "Offer",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_TagId",
                table: "Offer",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Category_CategoryId",
                table: "Offer",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Collection_CollectionId",
                table: "Offer",
                column: "CollectionId",
                principalTable: "Collection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Offer_Tag_TagId",
                table: "Offer",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
