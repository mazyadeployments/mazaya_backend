using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdatedEmailTemplateForInvitingNewFamilyMembers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE EmailTemplate set [Subject] = 'Invitation to Mazaya Offers', [Message] = 'You have been invited to join Mazaya Offers.
                                   Download today to avail amazing deals.
                                   <br /><br />
                                   Use this (@@custom-message@@) email to Register as per the steps below:' where [Name] like 'Adnoc_Employee_Invited_New_Family_Member'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
