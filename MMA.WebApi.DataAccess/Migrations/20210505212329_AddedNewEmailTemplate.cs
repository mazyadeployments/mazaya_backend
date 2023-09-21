using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedNewEmailTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                                    IF NOT EXISTS(SELECT 1 FROM EmailTemplate WHERE [Name] like 'Adnoc_Employee_Invited_New_Family_Member')
                                    BEGIN
	                                    DECLARE @EmailTemplateCount INT = (select COUNT(*) from EmailTemplate) + 1;
	                                    INSERT INTO EmailTemplate (Id, [Name], [Subject], Body, [Message], [Notification], NotificationTypeId, Sms, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
	                                    VALUES (@EmailTemplateCount , 'Adnoc_Employee_Invited_New_Family_Member', 'Invitation for Mazaya Offers', '', 'You have been invited to join Mazaya Offers.', '', 0, null, GETDATE(), 'sysadmin', GETDATE(), 'sysadmin')
                                    END"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
