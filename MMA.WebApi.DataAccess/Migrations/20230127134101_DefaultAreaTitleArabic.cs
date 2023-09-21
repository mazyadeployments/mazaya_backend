using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class DefaultAreaTitleArabic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TitleArabic",
                table: "DefaultArea",
                nullable: true);
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'أبو ظبي' where Title='Abu Dhabi'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'عجمان' where Title='Ajman'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'دبي' where Title='Dubai'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'الفجيرة' where Title='Fujairah'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'رأس الخيمة' where Title='Ras Al Khaimah'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'الشارقة' where Title='Sharjah'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'أم القيوين' where Title='Umm Al Quwain'");
            migrationBuilder.Sql("update DefaultArea set TitleArabic=N'العين' where Title='Al Ain'");



        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TitleArabic",
                table: "DefaultArea");
        }
    }
}
