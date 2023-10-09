using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MazayaPaymentgateways");

            migrationBuilder.DropColumn(
                name: "subcategoryids",
                table: "MazayaCategories");

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "MazayaSubcategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                table: "MazayaCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MazayaEcardmains",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    invoice_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_expire = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(28,12)", nullable: false),
                    vat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    grandtotal = table.Column<decimal>(type: "decimal(28,12)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    additionalcount = table.Column<int>(type: "int", nullable: false),
                    subcategoryids = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaEcardmains", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MazayaEcarddetails",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    relation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    card_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    profile_img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MazayaEcardmainid = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaEcarddetails", x => x.id);
                    table.ForeignKey(
                        name: "FK_MazayaEcarddetails_MazayaEcardmains_MazayaEcardmainid",
                        column: x => x.MazayaEcardmainid,
                        principalTable: "MazayaEcardmains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MazayaEcarddetails_MazayaEcardmainid",
                table: "MazayaEcarddetails",
                column: "MazayaEcardmainid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MazayaEcarddetails");

            migrationBuilder.DropTable(
                name: "MazayaEcardmains");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "MazayaSubcategories");

            migrationBuilder.DropColumn(
                name: "sort_order",
                table: "MazayaCategories");

            migrationBuilder.AddColumn<string>(
                name: "subcategoryids",
                table: "MazayaCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MazayaPaymentgateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(28,12)", nullable: false),
                    Bankref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cardname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cardno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cardtype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deviceid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Paystatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaPaymentgateways", x => x.Id);
                });
        }
    }
}
