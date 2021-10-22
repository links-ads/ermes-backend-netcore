using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Person_IsNewUser_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNewUser",
                table: "persons",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNewUser",
                table: "persons");
        }
    }
}
