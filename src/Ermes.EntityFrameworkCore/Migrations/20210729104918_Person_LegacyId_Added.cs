using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Person_LegacyId_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegacyId",
                table: "persons",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LegacyId",
                table: "persons");
        }
    }
}
