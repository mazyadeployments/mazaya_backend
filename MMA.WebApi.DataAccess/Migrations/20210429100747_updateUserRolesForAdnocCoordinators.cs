using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class updateUserRolesForAdnocCoordinators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @UserId NVARCHAR(100) = '';
                DECLARE @RoleId NVARCHAR(100) = (select Id from AspNetRoles where [Name] like 'AdnocCoordinator')



                IF EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName like 'aalrayssi@adnoc.ae')
                BEGIN
	                SET @UserId = (select Id from AspNetUsers where UserName like 'aalrayssi@adnoc.ae');
	                UPDATE AspNetUserRoles SET RoleId = @RoleId where UserId = @UserId;
                END


                IF EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName like 'noalqubaisi@adnoc.ae')
                BEGIN
	                SET @UserId = (select Id from AspNetUsers where UserName like 'noalqubaisi@adnoc.ae');
	                UPDATE AspNetUserRoles SET RoleId = @RoleId where UserId = @UserId;
                END


                IF EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName like 'hmuhairi@adnoc.ae')
                BEGIN
	                SET @UserId = (select Id from AspNetUsers where UserName like 'hmuhairi@adnoc.ae');
	                UPDATE AspNetUserRoles SET RoleId = @RoleId where UserId = @UserId;
                END


                IF EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName like 'ralriyami@adnoc.ae')
                BEGIN
	                SET @UserId = (select Id from AspNetUsers where UserName like 'ralriyami@adnoc.ae');
	                UPDATE AspNetUserRoles SET RoleId = @RoleId where UserId = @UserId;
                END


                IF EXISTS(SELECT 1 FROM AspNetUsers WHERE UserName like 'talalhosani@adnoc.ae')
                BEGIN
	                SET @UserId = (select Id from AspNetUsers where UserName like 'talalhosani@adnoc.ae');
	                UPDATE AspNetUserRoles SET RoleId = @RoleId where UserId = @UserId;
                END

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
