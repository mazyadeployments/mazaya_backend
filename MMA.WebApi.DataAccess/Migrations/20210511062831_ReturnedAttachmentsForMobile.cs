using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class ReturnedAttachmentsForMobile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This field needs to be updated (Last Synchronization Date)
            migrationBuilder.Sql(@"update Offer set UpdatedOn = GETUTCDATE() + 1 ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
