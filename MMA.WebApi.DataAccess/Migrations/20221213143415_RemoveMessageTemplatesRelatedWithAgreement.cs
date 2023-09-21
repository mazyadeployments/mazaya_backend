using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class RemoveMessageTemplatesRelatedWithAgreement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete from EmailTemplate where Id=7 or Id=8 or Id=9;");
        }

        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
