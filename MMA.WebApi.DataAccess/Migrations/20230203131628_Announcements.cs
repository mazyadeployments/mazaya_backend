using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class Announcements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        Status = table.Column<int>(nullable: false),
                        AllBuyers = table.Column<bool>(nullable: false),
                        AllSuppliers = table.Column<bool>(nullable: false),
                        SpecificBuyers = table.Column<bool>(nullable: false),
                        SpecificSuppliers = table.Column<bool>(nullable: false),
                        CountAllToSend = table.Column<int>(nullable: false),
                        CountFailed = table.Column<int>(nullable: false),
                        CountSent = table.Column<int>(nullable: false),
                        Message = table.Column<string>(nullable: true),
                        CreatedOn = table.Column<DateTime>(nullable: false),
                        CreatedBy = table.Column<string>(nullable: true),
                        UpdatedOn = table.Column<DateTime>(nullable: false),
                        UpdatedBy = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "AnnouncementAttachments",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        AnnouncementId = table.Column<int>(nullable: false),
                        DocumentId = table.Column<Guid>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementAttachments_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_AnnouncementAttachments_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AnnouncementSpecificBuyers",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        AnnouncementId = table.Column<int>(nullable: false),
                        BuyerType = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementSpecificBuyers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementSpecificBuyers_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "AnnouncementSpecificSuppliers",
                columns: table =>
                    new
                    {
                        Id = table
                            .Column<int>(nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        AnnouncementId = table.Column<int>(nullable: false),
                        SupplierCategory = table.Column<int>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnouncementSpecificSuppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnouncementSpecificSuppliers_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementAttachments_AnnouncementId",
                table: "AnnouncementAttachments",
                column: "AnnouncementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementAttachments_DocumentId",
                table: "AnnouncementAttachments",
                column: "DocumentId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSpecificBuyers_AnnouncementId",
                table: "AnnouncementSpecificBuyers",
                column: "AnnouncementId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AnnouncementSpecificSuppliers_AnnouncementId",
                table: "AnnouncementSpecificSuppliers",
                column: "AnnouncementId"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AnnouncementAttachments");

            migrationBuilder.DropTable(name: "AnnouncementSpecificBuyers");

            migrationBuilder.DropTable(name: "AnnouncementSpecificSuppliers");

            migrationBuilder.DropTable(name: "Announcements");
        }
    }
}
