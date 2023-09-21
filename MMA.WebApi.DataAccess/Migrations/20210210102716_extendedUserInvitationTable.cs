using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class extendedUserInvitationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Script for updating Offer Location to Default Area values
            migrationBuilder.Sql(@"DECLARE @DefaultAreaId INT = 1;
                                   DECLARE @Title NVARCHAR(50) = '';

                                   WHILE @DefaultAreaId < 8
                                   BEGIN
	                                   SET @Title = (select COALESCE(title, '') from DefaultArea where Id = @DefaultAreaId);
	                                   update OfferLocation set DefaultAreaId = @DefaultAreaId where LOWER(COALESCE(Vicinity, '')) like '%' + LOWER(@Title) + '%'
	                                   SET @DefaultAreaId = @DefaultAreaId + 1;
                                   END

                                   update OfferLocation set DefaultAreaId = 8 where Country <> 'United Arab Emirates'

                                   update OfferLocation set DefaultAreaId = 1 where Vicinity in (N'Al Ain', N'Al  Ain', N'Al Rahba', N'??? ???', N'??????', N'?????')
                                   update OfferLocation set DefaultAreaId = 3 where Vicinity in (N'???')
                                   update OfferLocation set DefaultAreaId = 4 where Vicinity in (N'Al Aqah', N'?????')
                                   update OfferLocation set DefaultAreaId = 5 where Vicinity in (N'Adhen Village', N'Al Jazirah Al Hamra', N'Ras Al-Khaimah')
                                   update OfferLocation set DefaultAreaId = 6 where Vicinity in (N'Kalba', N'???????') ");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "UserInvitations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "UserInvitations");
        }
    }
}
