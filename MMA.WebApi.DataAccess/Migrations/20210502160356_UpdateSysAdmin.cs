using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateSysAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                        DECLARE @SysAdminID NVARCHAR(100) = (select Id from AspNetUsers where UserName like 'sysadmin');
                        DECLARE @CoordinatorRoleId NVARCHAR(100) = (select Id from AspNetRoles where [Name] like 'AdnocCoordinator');


                        DELETE FROM AspNetUserRoles where UserId = @SysAdminID;

                        INSERT INTO AspNetUserRoles(UserId, RoleId) VALUES(@SysAdminID, @CoordinatorRoleId);

                        UPDATE AspNetUsers set Email = 'mazaya@adnoc.ae', NormalizedEmail = 'MAZAYA@ADNOC.AE' where UserName like 'sysadmin';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
