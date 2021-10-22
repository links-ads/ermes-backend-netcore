using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Report_Public_Contents_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "reports",
                nullable: true,
                defaultValue: "Submitted");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "reports",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "reports");
        }
    }
}
