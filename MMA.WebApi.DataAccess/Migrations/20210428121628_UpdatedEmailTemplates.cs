using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdatedEmailTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update EmailTemplate set [Subject] = 'Your Company Registration has been processed.', 
															 [Message] = 'Your Company Registration has been @@custom-message@@', 
															 [Notification] = 'Your Company Registration has been @@custom-message@@',
															 Body = ''
									where [Name] = 'Supplier_Processed_Notify_Supplier'


									update EmailTemplate set [Subject] = 'New Supplier to process.', 
															 [Message] = 'Supplier Registration @@custom-message@@ has been assigned for your kind review.', 
															 [Notification] = 'Supplier Registration @@custom-message@@ has been assigned for your kind review.',
															 Body = ''
									where [Name] = 'Supplier_Registration_Notify_Coordinator'


									update EmailTemplate set [Subject] = 'New Offer to process.', 
															 [Message] = 'Offer @@custom-message@@ has been assigned for your kind review.', 
															 [Notification] = 'Offer @@custom-message@@ has been assigned for your kind review.',
															 Body = ''
									where [Name] = 'Offer_To_Process_Notify_Reviewer'


									update EmailTemplate set [Subject] = 'Your Offer has been returned.', 
															 [Message] = 'Your Offer @@custom-message@@ has been reviewed, and returned to you with comments.', 
															 [Notification] = 'Your Offer @@custom-message@@ has been reviewed, and returned to you with comments.',
															 Body = ''
									where [Name] = 'Offer_Returned_Notify_SupplierAdminOrSupplier'


									update EmailTemplate set [Subject] = 'New Offer to review.', 
															 [Message] = 'Supplier Offer @@custom-message@@ has been assigned for your kind review.', 
															 [Notification] = 'Supplier Offer @@custom-message@@ has been assigned for your kind review.',
															 Body = ''
									where [Name] = 'Offer_To_Process_Notify_Coordinator'


									update EmailTemplate set [Subject] = 'Your Offer has been processed.', 
															 [Message] = 'Your Offer has been @@custom-message@@ ', 
															 [Notification] = 'Your Offer has been @@custom-message@@ ',
															 Body = ''
									where [Name] = 'Offer_Processed_Notify_SupplierAdminOrSupplier'


									update EmailTemplate set [Subject] = 'New Agreement to review.', 
															 [Message] = 'Supplier Agreement @@custom-message@@ has been assigned for your kind review.', 
															 [Notification] = 'Supplier Agreement @@custom-message@@ has been assigned for your kind review.',
															 Body = ''
									where [Name] = 'Offer_Agreement_To_Process_Notify_Coordinator'


									update EmailTemplate set [Subject] = 'Your Agreement has been returned.', 
															 [Message] = 'Your Agreement @@custom-message@@ has been reviewed, and returned to you with comments.', 
															 [Notification] = 'Your Agreement @@custom-message@@ has been reviewed, and returned to you with comments.',
															 Body = ''
									where [Name] = 'Offer_Agreement_Returned_Notify_SupplierAdminOrSupplier'


									update EmailTemplate set [Subject] = 'Your Agreement has been processed.', 
															 [Message] = 'Your Agreement has been @@custom-message@@', 
															 [Notification] = 'Your Agreement has been @@custom-message@@',
															 Body = ''
									where [Name] = 'Offer_Agreement_Processed_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Your Company has been invited to Roadshow.', 
															 [Message] = 'Your company has been invited to Roadshow @@custom-message@@.', 
															 [Notification] = 'Your company has been invited to Roadshow @@custom-message@@.',
															 Body = ''
									where [Name] = 'Company_Invited_To_Roadshow_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Roadshow published.', 
															 [Message] = 'Roadshow @@custom-message@@ has been published.', 
															 [Notification] = 'Roadshow @@custom-message@@ has been published.',
															 Body = ''
									where [Name] = 'Roadshow_Published_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Your Roadshow Invite has been approved.', 
															 [Message] = 'Your Roadshow Invite for @@custom-message@@ Roadshow has been approved.', 
															 [Notification] = 'Your Roadshow Invite for @@custom-message@@ Roadshow has been approved.',
															 Body = ''
									where [Name] = 'Roadshow_Approved_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Your Roadshow Invite has been returned.', 
															 [Message] = 'Your Roadshow Invite for @@custom-message@@ Roadshow has been returned.', 
															 [Notification] = 'Your Roadshow Invite for @@custom-message@@ Roadshow has been returned.',
															 Body = ''
									where [Name] = 'Roadshow_Returned_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Roadshow Invite is in renegotiation status.', 
															 [Message] = 'Roadshow Invite for @@custom-message@@ Roadshow is in renegotiation status.', 
															 [Notification] = 'Roadshow Invite for @@custom-message@@ Roadshow is in renegotiation status.',
															 Body = ''
									where [Name] = 'Roadshow_Invite_Renegotiation_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Roadshow has expired.', 
															 [Message] = 'Roadshow @@custom-message@@ has expired.', 
															 [Notification] = 'Roadshow @@custom-message@@ has expired.',
															 Body = ''
									where [Name] = 'Roadshow_Expired_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Roadshow has been cancelled.', 
															 [Message] = 'Roadshow @@custom-message@@ has been cancelled.', 
															 [Notification] = 'Roadshow @@custom-message@@ has been cancelled.',
															 Body = ''
									where [Name] = 'Roadshow_Cancelled_Notify_SupplierAdminOrSupplier'



									update EmailTemplate set [Subject] = 'Roadshow starts in 5 days!', 
															 [Message] = 'Roadshow @@custom-message@@ starts in 5 days!', 
															 [Notification] = 'Roadshow @@custom-message@@ starts in 5 days!',
															 Body = ''
									where [Name] = 'Roadshow_Starts_In_5_Days_Notify_Coordinator'



									update EmailTemplate set [Subject] = 'Roadshow starts in 1 days!', 
															 [Message] = 'Roadshow @@custom-message@@ starts in 1 days!', 
															 [Notification] = 'Roadshow @@custom-message@@ starts in 1 days!',
															 Body = ''
									where [Name] = 'Roadshow_Starts_In_1_Day_Notify_Coordinator'



									update EmailTemplate set [Subject] = 'Roadshow starts today!', 
															 [Message] = 'Roadshow @@custom-message@@ starts today!', 
															 [Notification] = 'Roadshow @@custom-message@@ starts today!',
															 Body = ''
									where [Name] = 'Roadshow_Starts_Today_Notify_SupplierAdminOrSupplier'


									IF NOT EXISTS(SELECT 1 FROM EmailTemplate WHERE [Name] like 'Roadshow_Unpublished_Notify_SupplierAdminOrSupplier')
									BEGIN
										DECLARE @EmailTemplateCount INT = (select COUNT(*) from EmailTemplate) + 1;
										insert into EmailTemplate(Id, [Name], [Subject], Body, [Message], [Notification], NotificationTypeId, Sms, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
																	values (@EmailTemplateCount, 'Roadshow_Unpublished_Notify_SupplierAdminOrSupplier', 'Roadshow unpublished.', '', 'Roadshow @@custom-message@@ has been unpublished.', 'Roadshow @@custom-message@@ has been unpublished.', 1, null, GETDATE(), 'sysadmin', GETDATE(), 'sysadmin') 
									END
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
