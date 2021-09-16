using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Ermes.Migrations
{
    public partial class MapRequest_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "map_requests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(maxLength: 10, nullable: false),
                    Duration = table.Column<NpgsqlRange<DateTime>>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    Hazard = table.Column<string>(nullable: true),
                    Layer = table.Column<string>(nullable: true),
                    Frequency = table.Column<int>(nullable: false),
                    DataTypeId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_map_requests_persons_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_requests_Code",
                table: "map_requests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_map_requests_CreatorUserId",
                table: "map_requests",
                column: "CreatorUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_requests");
        }
    }
}
