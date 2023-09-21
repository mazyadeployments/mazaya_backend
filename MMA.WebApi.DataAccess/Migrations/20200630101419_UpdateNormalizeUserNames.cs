using Microsoft.EntityFrameworkCore.Migrations;

namespace MMA.WebApi.DataAccess.Migrations
{
    public partial class UpdateNormalizeUserNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "268B8B27-C510-4558-B591-9BDFBF9FF76F",
                column: "NormalizedUserName",
                value: "coordinator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "58624BF9-4931-451B-89D4-D8E3F1E6BA59",
                column: "NormalizedUserName",
                value: "admin");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5C7B9785-5755-428A-B741-35A75150870C",
                column: "NormalizedUserName",
                value: "user2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6296B4B7-CC01-467C-A621-979F95E494A0",
                column: "NormalizedUserName",
                value: "user3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7649070E-B684-4F70-BBCB-8346AC796310",
                column: "NormalizedUserName",
                value: "user1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "76FCE130-6E57-418F-A71B-5ABC235F6AC4",
                column: "NormalizedUserName",
                value: "reviewer");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B6E82FB2-CAC5-43E4-BCF4-4156AA829D78",
                column: "NormalizedUserName",
                value: "superadmin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "268B8B27-C510-4558-B591-9BDFBF9FF76F",
                column: "NormalizedUserName",
                value: "Coordinator");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "58624BF9-4931-451B-89D4-D8E3F1E6BA59",
                column: "NormalizedUserName",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5C7B9785-5755-428A-B741-35A75150870C",
                column: "NormalizedUserName",
                value: "User 2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6296B4B7-CC01-467C-A621-979F95E494A0",
                column: "NormalizedUserName",
                value: "User 3");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7649070E-B684-4F70-BBCB-8346AC796310",
                column: "NormalizedUserName",
                value: "User 1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "76FCE130-6E57-418F-A71B-5ABC235F6AC4",
                column: "NormalizedUserName",
                value: "Reviewer");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "B6E82FB2-CAC5-43E4-BCF4-4156AA829D78",
                column: "NormalizedUserName",
                value: "Super Admin");
        }
    }
}
