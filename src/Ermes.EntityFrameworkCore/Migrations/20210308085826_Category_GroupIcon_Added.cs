using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Category_GroupIcon_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "category_translations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupIcon",
                table: "categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "category_translations");

            migrationBuilder.DropColumn(
                name: "GroupIcon",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "categories",
                type: "text",
                nullable: true);
        }
    }
}
