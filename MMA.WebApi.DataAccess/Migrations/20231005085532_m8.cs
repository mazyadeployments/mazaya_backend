using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class m8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MazayaPaymentgateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    response_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    card_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    card_holder_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payment_option = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    customer_ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    eci = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fort_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    response_msg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authorization_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchant_reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cust_email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bankref = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deviceid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cardname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cardno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(28,12)", nullable: false),
                    Paystatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transaction_id = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MazayaPaymentgateways", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MazayaPaymentgateways");
        }
    }
}
