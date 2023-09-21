using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoadshowProposal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    RoadshowDetails = table.Column<string>(nullable: true),
                    EquipmentItem = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    TermsAndCondition = table.Column<string>(nullable: true),
                    TermsAndConditionChecked = table.Column<bool>(nullable: false),
                    OfferEffectiveDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowProposal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowProposal_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOffer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(nullable: true),
                    RoadshowProposalId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    RoadshowDetails = table.Column<string>(nullable: true),
                    EquipmentItem = table.Column<string>(nullable: true),
                    PromotionCode = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOffer_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOffer_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferProposalDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowOfferProposalId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    RoadshowProposalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferProposalDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferProposalDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferProposalDocument_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferCategory",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferCategory", x => new { x.RoadshowOfferId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCategory_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferCollection",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    CollectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferCollection", x => new { x.RoadshowOfferId, x.CollectionId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCollection_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferCollection_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowOfferId = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_RoadshowOfferDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferDocument_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferRating",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    CommentText = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferRating", x => new { x.RoadshowOfferId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferRating_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferRating_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowOfferTag",
                columns: table => new
                {
                    RoadshowOfferId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowOfferTag", x => new { x.RoadshowOfferId, x.TagId });
                    table.ForeignKey(
                        name: "FK_RoadshowOfferTag_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowOfferTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowVoucher",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(nullable: false),
                    Validity = table.Column<bool>(nullable: false),
                    Details = table.Column<string>(nullable: true),
                    RoadshowOfferId = table.Column<int>(nullable: true),
                    RoadshowProposalId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowVoucher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowVoucher_RoadshowOffer_RoadshowOfferId",
                        column: x => x.RoadshowOfferId,
                        principalTable: "RoadshowOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadshowVoucher_RoadshowProposal_RoadshowProposalId",
                        column: x => x.RoadshowProposalId,
                        principalTable: "RoadshowProposal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_CompanyId",
                table: "RoadshowOffer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_RoadshowProposalId",
                table: "RoadshowOffer",
                column: "RoadshowProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferCategory_CategoryId",
                table: "RoadshowOfferCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferCollection_CollectionId",
                table: "RoadshowOfferCollection",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferDocument_DocumentId",
                table: "RoadshowOfferDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferDocument_RoadshowOfferId",
                table: "RoadshowOfferDocument",
                column: "RoadshowOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferProposalDocument_DocumentId",
                table: "RoadshowOfferProposalDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferProposalDocument_RoadshowProposalId",
                table: "RoadshowOfferProposalDocument",
                column: "RoadshowProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferRating_ApplicationUserId",
                table: "RoadshowOfferRating",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOfferTag_TagId",
                table: "RoadshowOfferTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowProposal_CompanyId",
                table: "RoadshowProposal",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowOfferId",
                table: "RoadshowVoucher",
                column: "RoadshowOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowVoucher_RoadshowProposalId",
                table: "RoadshowVoucher",
                column: "RoadshowProposalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoadshowOfferCategory");

            migrationBuilder.DropTable(
                name: "RoadshowOfferCollection");

            migrationBuilder.DropTable(
                name: "RoadshowOfferDocument");

            migrationBuilder.DropTable(
                name: "RoadshowOfferProposalDocument");

            migrationBuilder.DropTable(
                name: "RoadshowOfferRating");

            migrationBuilder.DropTable(
                name: "RoadshowOfferTag");

            migrationBuilder.DropTable(
                name: "RoadshowVoucher");

            migrationBuilder.DropTable(
                name: "RoadshowOffer");

            migrationBuilder.DropTable(
                name: "RoadshowProposal");
        }
    }
}
