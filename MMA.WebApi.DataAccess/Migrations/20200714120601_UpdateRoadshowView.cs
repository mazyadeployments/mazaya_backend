using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateRoadshowView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (@"
                CREATE OR ALTER VIEW RoadshowView
                AS
                SELECT rlo.RoadshowId as RoadshowId, rlo.LocationId as LocationId, rlo.OfferId as OfferId, 
					   loc.City as City, loc.RoadShowStartDate as RoadShowStartDate, loc.RoadShowEndDate as RoadShowEndDate,
					   offer.OfferTitle as OfferTitle, offer.OfferDescription as OfferDescription,
					   cat.CategoryTitle as CategoryTitle,
					   cou.CountyName as CountryName,
					   document.DocumentId as DocumentId, document.ImageType as ImageType,
					   roadshow.Active as RoadshowActive				   
					from [RoadshowLocationOffers] rlo
					INNER JOIN (SELECT Id as LocationId, City as City, RoadShowStartDate as RoadShowStartDate, RoadShowEndDate as RoadShowEndDate, CountryId as CountryId FROM Location) as loc
						ON loc.LocationId = rlo.LocationId
					INNER JOIN (SELECT Id as RoadshowId, Active as Active FROM Roadshow) as roadshow
						ON roadshow.RoadshowId = rlo.RoadshowId
					INNER JOIN (SELECT Id as OfferId, Title as OfferTitle, Description as OfferDescription FROM Offer) as offer
						ON offer.OfferId = rlo.OfferId
					INNER JOIN (SELECT OfferId as OfferId, CategoryId as CategoryId FROM OfferCategory) as offercategory
						ON offer.OfferId = rlo.OfferId
					INNER JOIN (SELECT Id as CategoryId, Title as CategoryTitle FROM Category) as cat
						ON cat.CategoryId = offercategory.CategoryId
					INNER JOIN (SELECT Id as CountyId, Name as CountyName FROM Country) as cou
						ON cou.CountyId = loc.CountryId
					INNER JOIN (SELECT o.Id as OfferId, d.Id as DocumentId, od.Type as ImageType FROM Offer o
								INNER JOIN OfferDocument od 
									ON od.OfferId = o.Id
								INNER JOIN Document d
								ON d.Id = od.DocumentId) as document
						ON  document.OfferId = rlo.OfferId

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW RoadshowView");
        }
    }
}
