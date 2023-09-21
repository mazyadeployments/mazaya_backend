using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RoadshowModelDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoadshowEventId",
                table: "RoadshowOffer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roadshow",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roadshow", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowDocument",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowDocument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowDocument_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowInvite",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    RoadshowId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowInvite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowInvite_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadshowInvite_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longitude = table.Column<string>(nullable: true),
                    Latitude = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    RoadshowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowLocation_Roadshow_RoadshowId",
                        column: x => x.RoadshowId,
                        principalTable: "Roadshow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowComment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    RoadshowId = table.Column<int>(nullable: false),
                    RoadshowInviteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowComment_RoadshowInvite_RoadshowInviteId",
                        column: x => x.RoadshowInviteId,
                        principalTable: "RoadshowInvite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoadshowEvent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoadshowLocationId = table.Column<int>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    RoadshowInviteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadshowEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadshowEvent_RoadshowInvite_RoadshowInviteId",
                        column: x => x.RoadshowInviteId,
                        principalTable: "RoadshowInvite",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoadshowEvent_RoadshowLocation_RoadshowLocationId",
                        column: x => x.RoadshowLocationId,
                        principalTable: "RoadshowLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowOffer_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowComment_RoadshowInviteId",
                table: "RoadshowComment",
                column: "RoadshowInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowDocument_DocumentId",
                table: "RoadshowDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowDocument_RoadshowId",
                table: "RoadshowDocument",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowInviteId",
                table: "RoadshowEvent",
                column: "RoadshowInviteId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowEvent_RoadshowLocationId",
                table: "RoadshowEvent",
                column: "RoadshowLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowInvite_CompanyId",
                table: "RoadshowInvite",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowInvite_RoadshowId",
                table: "RoadshowInvite",
                column: "RoadshowId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadshowLocation_RoadshowId",
                table: "RoadshowLocation",
                column: "RoadshowId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer",
                column: "RoadshowEventId",
                principalTable: "RoadshowEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoadshowOffer_RoadshowEvent_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.DropTable(
                name: "RoadshowComment");

            migrationBuilder.DropTable(
                name: "RoadshowDocument");

            migrationBuilder.DropTable(
                name: "RoadshowEvent");

            migrationBuilder.DropTable(
                name: "RoadshowInvite");

            migrationBuilder.DropTable(
                name: "RoadshowLocation");

            migrationBuilder.DropTable(
                name: "Roadshow");

            migrationBuilder.DropIndex(
                name: "IX_RoadshowOffer_RoadshowEventId",
                table: "RoadshowOffer");

            migrationBuilder.DropColumn(
                name: "RoadshowEventId",
                table: "RoadshowOffer");
        }
    }
}
