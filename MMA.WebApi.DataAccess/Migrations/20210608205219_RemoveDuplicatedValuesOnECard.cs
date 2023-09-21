using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RemoveDuplicatedValuesOnECard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers
                SET ECardSequence = NULL
                WHERE EXISTS (SELECT ECardSequence FROM AspNetUsers u2
                              WHERE AspNetUsers.ECardSequence = u2.ECardSequence
                              GROUP BY u2.ECardSequence
                              HAVING COUNT(*) > 1)
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
