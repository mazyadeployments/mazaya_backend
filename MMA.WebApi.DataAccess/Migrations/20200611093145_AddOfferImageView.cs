using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddOfferImageView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (@"
                CREATE VIEW OfferImages
                AS
                SELECT o.Id as OfferId, d.Id as DocumentId, od.[Type] as ImageType FROM Offer o
	                INNER JOIN OfferDocument od 
		                ON od.OfferId = o.Id
	                INNER JOIN Document d
		                ON d.Id = od.DocumentId
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW OfferImages");
        }
    }
}
