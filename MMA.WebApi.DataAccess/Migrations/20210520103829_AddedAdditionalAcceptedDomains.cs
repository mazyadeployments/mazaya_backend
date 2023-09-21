using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedAdditionalAcceptedDomains : Migration
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


									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@adnoc.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@adnoc.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@borouge.com')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@borouge.com')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@adnocdistribution.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@adnocdistribution.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@rationaletech.com')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@rationaletech.com')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@spc.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@spc.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@adpolice.gov.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@adpolice.gov.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@fertil.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@fertil.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@fertil.com')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@fertil.com')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@ata.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@ata.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@etihad.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@etihad.ae')
									END

									IF NOT EXISTS (SELECT * FROM AcceptedDomain WHERE Domain like '@htm.ae')
									BEGIN
										insert into AcceptedDomain(Id, Domain)
										values (CONVERT(varchar(255), NEWID()), '@htm.ae')
									END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
