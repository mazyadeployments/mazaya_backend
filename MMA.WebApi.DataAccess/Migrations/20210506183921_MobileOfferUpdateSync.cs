using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class MobileOfferUpdateSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update Offer set UpdatedOn = GETDATE() ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
