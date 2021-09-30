using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class LayerDefinition_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "layers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataTypeId = table.Column<int>(nullable: false),
                    GroupKey = table.Column<string>(maxLength: 255, nullable: false),
                    SubGroupKey = table.Column<string>(maxLength: 255, nullable: false),
                    PartnerName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_layers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "layer_translations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Group = table.Column<string>(maxLength: 255, nullable: false),
                    SubGroup = table.Column<string>(maxLength: 255, nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_layer_translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_layer_translations_layers_CoreId",
                        column: x => x.CoreId,
                        principalTable: "layers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_layer_translations_CoreId_Language",
                table: "layer_translations",
                columns: new[] { "CoreId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_layers_DataTypeId",
                table: "layers",
                column: "DataTypeId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "layer_translations");

            migrationBuilder.DropTable(
                name: "layers");
        }
    }
}
