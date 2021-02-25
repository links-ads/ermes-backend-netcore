using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Property_Visibility_Removed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "reportrequests");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "person_actions");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "missions");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "communications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "reportrequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "person_actions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "missions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "communications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
