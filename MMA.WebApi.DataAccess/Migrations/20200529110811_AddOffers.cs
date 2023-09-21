using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 40, nullable: false),
                    Description = table.Column<string>(maxLength: 10000, nullable: true),
                    CategoryId = table.Column<int>(nullable: true),
                    CollectionId = table.Column<int>(nullable: true),
                    Tag = table.Column<string>(maxLength: 10000, nullable: true),
                    PromotionCode = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(maxLength: 500, nullable: true),
                    City = table.Column<string>(maxLength: 100, nullable: true),
                    Country = table.Column<string>(maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidUntil = table.Column<DateTime>(nullable: true),
                    WhatYouGet = table.Column<string>(maxLength: 10000, nullable: true),
                    PriceList = table.Column<string>(maxLength: 10000, nullable: true),
                    TermsAndCondition = table.Column<string>(maxLength: 10000, nullable: true),
                    AboutCompany = table.Column<string>(maxLength: 10000, nullable: true),
                    Longtitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offer_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offer_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CategoryId",
                table: "Offer",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offer_CollectionId",
                table: "Offer",
                column: "CollectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offer");
        }
    }
}
