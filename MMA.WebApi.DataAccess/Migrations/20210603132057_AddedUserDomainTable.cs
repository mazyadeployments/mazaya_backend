using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedUserDomainTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove all unspecified user types
            migrationBuilder.Sql(@"update AspNetUsers set UserType = 7 where UserType is null or UserType not in (1, 2, 3, 4, 5, 6, 7) ");
            migrationBuilder.Sql(@"update UserInvitations set UserType = 7 where UserType is null or UserType not in (1, 2, 3, 4, 5, 6, 7) ");
            // Remove all duplicates on ECard
            migrationBuilder.Sql(@"
                UPDATE AspNetUsers
                SET ECardSequence = NULL
                WHERE EXISTS (SELECT ECardSequence FROM AspNetUsers u2
                              WHERE AspNetUsers.ECardSequence = u2.ECardSequence
                              GROUP BY u2.ECardSequence
                              HAVING COUNT(*) > 1)
            ");

            migrationBuilder.CreateTable(
                name: "UserDomain",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DomainName = table.Column<string>(nullable: true),
                    KeyValue = table.Column<string>(nullable: true),
                    Domains = table.Column<string>(nullable: true),
                    SequencerName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDomain", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserDomain",
                columns: new[] { "Id", "DomainName", "Domains", "KeyValue", "SequencerName" },
                values: new object[,]
                {
                    { 1, "ADNOCEmployee", "@adnoc;", "1971", "dbo.ADNOCEmployeeSequencer" },
                    { 2, "ADNOCEmployeeFamilyMember", "", "1971", "dbo.ADNOCEmployeeSequencer" },
                    { 3, "ADPolice", "", "1957", "dbo.ADPoliceSequencer" },
                    { 4, "RedCrescent", "", "1983", "dbo.RedCrescentSequencer" },
                    { 5, "AlumniRetirementMembers", "", "2018", "dbo.AlumniRetirementMembersSequencer" },
                    { 6, "ADSchools", "", "9999", "dbo.DefaultSequencer" },
                    { 7, "Other", "", "0000", "dbo.DefaultSequencer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserInvitations_UserType",
                table: "UserInvitations",
                column: "UserType");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers",
                column: "UserType");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserDomain_UserType",
                table: "AspNetUsers",
                column: "UserType",
                principalTable: "UserDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInvitations_UserDomain_UserType",
                table: "UserInvitations",
                column: "UserType",
                principalTable: "UserDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);


            // Add Default Sequencer
            migrationBuilder.Sql
            (@"
                    IF NOT EXISTS
                    (
	                    select 1
	                    from sys.sequences
	                    where object_id = object_id('dbo.DefaultSequencer')
                    )
                    BEGIN
                        CREATE SEQUENCE DefaultSequencer
                                        START WITH 1
                                        INCREMENT BY 1
                                        MINVALUE 0
                                        MAXVALUE 9999999999;
                    END
            ");


            migrationBuilder.Sql
            (@"
               IF NOT EXISTS
                (
	                SELECT 1 FROM AspNetUsers 
	                WHERE EXISTS (SELECT ECardSequence FROM AspNetUsers u2
					                WHERE AspNetUsers.ECardSequence = u2.ECardSequence
					                GROUP BY u2.ECardSequence
					                HAVING COUNT(*) > 1)
                )
                BEGIN 
	                 IF NOT EXISTS
                    (
                        SELECT 1 FROM sys.indexes
                        WHERE name='idx_eCard_notnull' AND object_id = OBJECT_ID('dbo.AspNetUsers')
                    )
                    BEGIN
                        ALTER TABLE AspNetUsers ALTER COLUMN ECardSequence nvarchar(200)

                        create unique nonclustered index idx_eCard_notnull
                        on AspNetUsers(ECardSequence)
                        where ECardSequence is not null;
                    END
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserDomain_UserType",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInvitations_UserDomain_UserType",
                table: "UserInvitations");

            migrationBuilder.DropTable(
                name: "UserDomain");

            migrationBuilder.DropIndex(
                name: "IX_UserInvitations_UserType",
                table: "UserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserType",
                table: "AspNetUsers");
        }
    }
}
