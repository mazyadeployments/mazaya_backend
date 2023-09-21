using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class MobileOfferUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update Offer set UpdatedOn = GETUTCDATE() ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
