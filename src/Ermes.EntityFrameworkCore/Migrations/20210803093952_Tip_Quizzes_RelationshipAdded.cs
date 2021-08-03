using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Tip_Quizzes_RelationshipAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_quizzes_TipCode",
                table: "quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_TipCode",
                table: "quizzes",
                column: "TipCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_quizzes_TipCode",
                table: "quizzes");

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_TipCode",
                table: "quizzes",
                column: "TipCode",
                unique: false);
        }
    }
}
