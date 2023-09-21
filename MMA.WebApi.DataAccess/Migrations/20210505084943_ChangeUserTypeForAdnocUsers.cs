using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class ChangeUserTypeForAdnocUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE AspNetUsers SET UserType = 1 WHERE Email LIKE '%@adnoc.ae%' ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
