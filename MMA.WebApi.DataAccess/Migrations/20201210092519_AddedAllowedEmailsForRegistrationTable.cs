using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedAllowedEmailsForRegistrationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowedEmailsForRegistration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 1000, nullable: true),
                    UserType = table.Column<int>(nullable: false),
                    InviteSent = table.Column<bool>(nullable: false),
                    InviteSentOn = table.Column<DateTime>(nullable: false),
                    InviteSentBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedEmailsForRegistration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedEmailsForRegistration");
        }
    }
}
