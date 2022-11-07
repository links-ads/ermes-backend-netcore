using System.Collections.Generic;
using Ermes.MapRequests;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class MapRequest_Layer_ErrorMessages_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "map_request_layers");

            migrationBuilder.AddColumn<List<MapRequestLayerError>>(
                name: "ErrorMessages",
                table: "map_request_layers",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedUpdates",
                table: "map_request_layers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"update public.map_request_layers set ""Status"" = 'Completed' where ""Status"" = 'Error';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessages",
                table: "map_request_layers");

            migrationBuilder.DropColumn(
                name: "ReceivedUpdates",
                table: "map_request_layers");

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "map_request_layers",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
