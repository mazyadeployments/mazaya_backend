using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateApproveStatusForMigratedCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update Company set ApproveStatus = 'Approved' where ApproveStatus is null");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
