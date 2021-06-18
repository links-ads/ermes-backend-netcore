using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Category_Target_Translations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Target",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "category_translations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetKey",
                table: "categories",
                nullable: false,
                defaultValue: "None");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Target",
                table: "category_translations");

            migrationBuilder.DropColumn(
                name: "TargetKey",
                table: "categories");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "categories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
