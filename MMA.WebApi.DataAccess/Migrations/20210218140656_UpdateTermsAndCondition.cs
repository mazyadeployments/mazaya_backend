using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateTermsAndCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            // Script for Terms & Conditions for offers
            migrationBuilder.Sql(@"DECLARE @TermsAndConditionsForOffer AS NVARCHAR(MAX) = (select top 1 Content from AdnocTermsAndConditions where [Type] = 4 order by CreatedOn desc)
                                   DECLARE @AdminId NVARCHAR(50) = (select top 1 Id from AspNetUserRoles ur
								                                    inner join AspNetRoles r on ur.RoleId = r.Id
								                                    where r.[Name] = 'Admin')

                                   UPDATE Offer SET TermsAndCondition = @TermsAndConditionsForOffer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
