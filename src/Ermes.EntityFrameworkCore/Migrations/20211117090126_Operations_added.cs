using System;
using Ermes.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Operations_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentOperationLegacyId",
                table: "persons",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "volter_operations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    PersonLegacyId = table.Column<int>(nullable: false),
                    OperationLegacyId = table.Column<int>(nullable: false),
                    Request = table.Column<string>(type: "jsonb", nullable: true),
                    Response = table.Column<VolterResponse>(type: "jsonb", nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volter_operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_volter_operations_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_volter_operations_PersonId",
                table: "volter_operations",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "volter_operations");

            migrationBuilder.DropColumn(
                name: "CurrentOperationLegacyId",
                table: "persons");
        }
    }
}
