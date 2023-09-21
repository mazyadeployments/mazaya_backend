using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedDefaultLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (@"
               INSERT INTO [DefaultLocation] (Title, Longitude, Latitude, Address, Vicinity, Country)
                    VALUES ('HQ', '54.32486859999999', '24.46183539999999', 'King Salman Bin Abdulaziz Al Saud St, Al Bateen, Abu Dhabi, United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates'),
                           ('Ruwais', '52.649124', '24.068093', 'Al Ruwais - Abu Dhabi, United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates'),
	                       ('SKEC 2', '54.3642683', '24.4961172', '112 Fatima Bint Mubarak St, Zone 1E7, Abu Dhabi, United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates'),
	                       ('ATA', '54.685012', '24.379280', '112 Fatima Bint Mubarak St, Zone 1E7, Abu Dhabi, United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates'),
	                       ('Onshore', '54.343254', '24.473566', 'Corniche Rd W - Al KhalidiyahW8 - Abu Dhabi - United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates'),
	                       ('Offshore', '54.364290', '24.496306', 'Unnamed Road - Zone 1شرق - 7th St - Abu Dhabi - United Arab Emirates', 'Abu Dhabi', 'United Arab Emirates');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM DefaultLocation");
        }
    }
}
