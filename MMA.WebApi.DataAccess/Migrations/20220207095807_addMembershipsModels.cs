using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class addMembershipsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipPictureDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DocumentIdHorizontalPicture = table.Column<Guid>(nullable: false),
                    DocumentIdHorizontalBackPicture = table.Column<Guid>(nullable: false),
                    DocumentIdVerticalPicture = table.Column<Guid>(nullable: false),
                    DocumentIdVerticalBackPicture = table.Column<Guid>(nullable: false),
                    MembershipType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPictureDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NameEng = table.Column<string>(nullable: true),
                    NameAr = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PictureDataId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memberships_MembershipPictureDatas_PictureDataId",
                        column: x => x.PictureDataId,
                        principalTable: "MembershipPictureDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembershipECards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<string>(nullable: true),
                    MemberId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    ECardSequence = table.Column<string>(nullable: true),
                    MembershipType = table.Column<int>(nullable: false),
                    IsMembershipCard = table.Column<bool>(nullable: false),
                    PhotoUrl = table.Column<string>(nullable: true),
                    isMember = table.Column<bool>(nullable: false),
                    MembershipId = table.Column<Guid>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipECards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembershipECards_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MembershipECards_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OffersMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OfferId = table.Column<int>(nullable: false),
                    MembershipId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffersMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OffersMemberships_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffersMemberships_Offer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipECards_MembershipId",
                table: "MembershipECards",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipECards_OwnerId",
                table: "MembershipECards",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_PictureDataId",
                table: "Memberships",
                column: "PictureDataId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersMemberships_MembershipId",
                table: "OffersMemberships",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_OffersMemberships_OfferId",
                table: "OffersMemberships",
                column: "OfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipECards");

            migrationBuilder.DropTable(
                name: "OffersMemberships");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "MembershipPictureDatas");
        }
    }
}
