using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Layer_Format_CanBeVisualized_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanBeVisualized",
                table: "layers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "layers",
                nullable: true,
                defaultValue: "GeoJSON");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanBeVisualized",
                table: "layers");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "layers");
        }
    }
}
