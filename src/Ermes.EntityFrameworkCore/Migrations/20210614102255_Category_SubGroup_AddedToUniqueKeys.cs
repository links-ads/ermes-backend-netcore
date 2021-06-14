using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Category_SubGroup_AddedToUniqueKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_category_translations_Group_Name_Language",
                table: "category_translations");

            migrationBuilder.CreateIndex(
                name: "IX_category_translations_Group_SubGroup_Name_Language",
                table: "category_translations",
                columns: new[] { "Group", "SubGroup", "Name", "Language" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_category_translations_Group_SubGroup_Name_Language",
                table: "category_translations");

            migrationBuilder.CreateIndex(
                name: "IX_category_translations_Group_Name_Language",
                table: "category_translations",
                columns: new[] { "Group", "Name", "Language" },
                unique: true);
        }
    }
}
