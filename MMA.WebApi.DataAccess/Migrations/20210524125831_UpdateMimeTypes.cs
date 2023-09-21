using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateMimeTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update Document set MimeType = 'application/pdf' where MimeType like '%octet%' and [Name] is null
                update Document set MimeType = 'application/pdf' where MimeType like '%octet%' and [Name] like '%.pdf'
                update Document set MimeType = 'image/png' where MimeType like '%octet%' and [Name] like '%.png'
                update Document set MimeType = 'image/jpeg' where MimeType like '%octet%' and [Name] like '%.jpg'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
