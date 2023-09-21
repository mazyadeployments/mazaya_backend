using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class InsertNewDomains : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@ata.ac.ae')
                                    BEGIN
	                                    insert into AcceptedDomain(Id, Domain)
	                                    values (CONVERT(varchar(255), NEWID()), '@ata.ac.ae')
                                    END

                                    IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@etihad.com')
                                    BEGIN
	                                    insert into AcceptedDomain(Id, Domain)
	                                    values (CONVERT(varchar(255), NEWID()), '@etihad.com')
                                    END

                                    IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@pension.gov.ae')
                                    BEGIN
	                                    insert into AcceptedDomain(Id, Domain)
	                                    values (CONVERT(varchar(255), NEWID()), '@pension.gov.ae')
                                    END

                                    IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@rcuae.ae')
                                    BEGIN
	                                    insert into AcceptedDomain(Id, Domain)
	                                    values (CONVERT(varchar(255), NEWID()), '@rcuae.ae')
                                    END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
