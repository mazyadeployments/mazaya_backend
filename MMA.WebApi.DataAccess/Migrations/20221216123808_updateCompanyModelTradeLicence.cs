using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class updateCompanyModelTradeLicence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MemberEmail",
                table: "MembershipECards",
                nullable: true
            );

            migrationBuilder.AlterColumn<Guid>(
                name: "TradeLicenceId",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Company_TradeLicenceId",
                table: "Company",
                column: "TradeLicenceId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Document_TradeLicenceId",
                table: "Company",
                column: "TradeLicenceId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Document_TradeLicenceId",
                table: "Company"
            );

            migrationBuilder.DropIndex(name: "IX_Company_TradeLicenceId", table: "Company");

            migrationBuilder.DropColumn(name: "MemberEmail", table: "MembershipECards");

            migrationBuilder.AlterColumn<string>(
                name: "TradeLicenceId",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true
            );
        }
    }
}
