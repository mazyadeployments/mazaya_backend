using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class AddedCompanyLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierId",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Longtitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(28, 12)", nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Vicinity = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLocation_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLocation_CompanyId",
                table: "CompanyLocation",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyLocation");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Company");
        }
    }
}
