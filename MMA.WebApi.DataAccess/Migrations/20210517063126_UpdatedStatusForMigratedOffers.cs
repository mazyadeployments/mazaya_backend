using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdatedStatusForMigratedOffers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update offer set [Status] = 'Approved' where [Status] like 'Migrated' ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
