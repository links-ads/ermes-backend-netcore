using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Frequency_Layer_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "layers",
                nullable: true,
                defaultValue: "H6");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "layers");
        }
    }
}
