using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class FixForDeltaMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
									-- Get all companies that are created on day of migration
									DECLARE @DateFrom NVARCHAR(100) = '2021-05-09';
									DECLARE @DateTo NVARCHAR(100) = '2021-05-11';
									DECLARE @CompanyCount INT = (select COUNT(*) from Company where CreatedOn > @DateFrom and CreatedOn < @DateTo);
									DECLARE @Counter INT = 1;

									-- Get SysAdminId
									DECLARE @AdminId NVARCHAR(100) = (select Id from AspNetUsers where UserName like 'sysadmin');
									-- SupplierAdminId
									DECLARE @SupplierAdminId NVARCHAR(100) = '';
									-- CompanyId
									DECLARE @CompanyId INT = 0;

									-- All companies Ids
									select Id, ROW_NUMBER() over (order by Id) as rowNum 
									into #companies
									from Company
									where CreatedOn > @DateFrom and CreatedOn < @DateTo and CreatedBy like @AdminId

									-- For each company execute this script
									WHILE (@Counter <= @CompanyCount)
									BEGIN
										-- Set company
										SET @CompanyId = (select Id from #companies where rowNum = @Counter)
										-- Set supplier admin id
										SET @SupplierAdminId = (select top 1 SupplierId from CompanySuppliers where CompanyId = @CompanyId)

										IF @SupplierAdminId <> @AdminId
										BEGIN
											-- delete all company suppliers for that company
											delete from CompanySuppliers where CompanyId = @CompanyId;
											-- set admin as company supplier
											insert into CompanySuppliers (CompanyId, SupplierId) values (@CompanyId, @AdminId)

											-- delete supplier admin user from system
											delete from ApplicationUserDocument where ApplicationUserId = @SupplierAdminId;
											delete from MailStorage where UserId = @SupplierAdminId;
											delete from UserNotification where UserId = @SupplierAdminId;
											delete from AspNetUserRoles where UserId = @SupplierAdminId;
											delete from AspNetUsers where Id = @SupplierAdminId;
										END

										SET @Counter  = @Counter + 1
									END

									-- Drop temp table
									drop table #companies;
            
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
