using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Answers_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_quizzes_Code",
                table: "quizzes",
                column: "Code");

            migrationBuilder.CreateTable(
                name: "answers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    QuizCode = table.Column<string>(nullable: true),
                    IsTheRightAnswer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_answers_quizzes_QuizCode",
                        column: x => x.QuizCode,
                        principalTable: "quizzes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "answer_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Text = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_answer_translations_answers_CoreId",
                        column: x => x.CoreId,
                        principalTable: "answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answer_translations_CoreId_Language",
                table: "answer_translations",
                columns: new[] { "CoreId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_answers_Code",
                table: "answers",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_answers_QuizCode",
                table: "answers",
                column: "QuizCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer_translations");

            migrationBuilder.DropTable(
                name: "answers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_quizzes_Code",
                table: "quizzes");
        }
    }
}
