using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Tips_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tips",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Hazard = table.Column<string>(nullable: false),
                    CrisisPhase = table.Column<string>(nullable: false),
                    EventContext = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tip_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    Text = table.Column<string>(maxLength: 1000, nullable: false),
                    CrisisPhase = table.Column<string>(maxLength: 50, nullable: false),
                    EventContext = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tip_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tip_translations_tips_CoreId",
                        column: x => x.CoreId,
                        principalTable: "tips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tip_translations_CoreId",
                table: "tip_translations",
                column: "CoreId");

            migrationBuilder.CreateIndex(
                name: "IX_tips_Code",
                table: "tips",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tip_translations");

            migrationBuilder.DropTable(
                name: "tips");
        }
    }
}
