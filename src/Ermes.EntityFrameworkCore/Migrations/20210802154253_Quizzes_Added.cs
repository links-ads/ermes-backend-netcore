using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Quizzes_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tip_translations_CoreId",
                table: "tip_translations");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_tips_Code",
                table: "tips",
                column: "Code");

            migrationBuilder.CreateTable(
                name: "quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    TipCode = table.Column<string>(nullable: true),
                    Hazard = table.Column<string>(nullable: false),
                    CrisisPhase = table.Column<string>(nullable: false),
                    EventContext = table.Column<string>(nullable: false),
                    Difficulty = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quizzes_tips_TipCode",
                        column: x => x.TipCode,
                        principalTable: "tips",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quiz_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Text = table.Column<string>(maxLength: 1000, nullable: false),
                    CrisisPhase = table.Column<string>(maxLength: 50, nullable: false),
                    EventContext = table.Column<string>(maxLength: 50, nullable: false),
                    Difficulty = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quiz_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quiz_translations_quizzes_CoreId",
                        column: x => x.CoreId,
                        principalTable: "quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tip_translations_CoreId_Language",
                table: "tip_translations",
                columns: new[] { "CoreId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quiz_translations_CoreId_Language",
                table: "quiz_translations",
                columns: new[] { "CoreId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_Code",
                table: "quizzes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quizzes_TipCode",
                table: "quizzes",
                column: "TipCode",
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quiz_translations");

            migrationBuilder.DropTable(
                name: "quizzes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_tips_Code",
                table: "tips");

            migrationBuilder.DropIndex(
                name: "IX_tip_translations_CoreId_Language",
                table: "tip_translations");

            migrationBuilder.CreateIndex(
                name: "IX_tip_translations_CoreId",
                table: "tip_translations",
                column: "CoreId");
        }
    }
}
