using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class MapRequestLayers_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataTypeIds",
                table: "map_requests");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_map_requests_Code",
                table: "map_requests",
                column: "Code");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_layers_DataTypeId",
                table: "layers",
                column: "DataTypeId");

            migrationBuilder.CreateTable(
                name: "map_request_layers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LayerDataTypeId = table.Column<int>(nullable: false),
                    MapRequestCode = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    ErrorMessage = table.Column<string>(maxLength: 2000, nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_request_layers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_map_request_layers_layers_LayerDataTypeId",
                        column: x => x.LayerDataTypeId,
                        principalTable: "layers",
                        principalColumn: "DataTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_map_request_layers_map_requests_MapRequestCode",
                        column: x => x.MapRequestCode,
                        principalTable: "map_requests",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_map_request_layers_LayerDataTypeId",
                table: "map_request_layers",
                column: "LayerDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_map_request_layers_MapRequestCode_LayerDataTypeId",
                table: "map_request_layers",
                columns: new[] { "MapRequestCode", "LayerDataTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_request_layers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_map_requests_Code",
                table: "map_requests");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_layers_DataTypeId",
                table: "layers");

            migrationBuilder.AddColumn<List<int>>(
                name: "DataTypeIds",
                table: "map_requests",
                type: "integer[]",
                nullable: true);
        }
    }
}
