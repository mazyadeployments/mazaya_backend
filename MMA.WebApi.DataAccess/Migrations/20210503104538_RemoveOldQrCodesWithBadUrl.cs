using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RemoveOldQrCodesWithBadUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from OfferDocument where [Type] = 5"); //Type 5 -> QRCode
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
