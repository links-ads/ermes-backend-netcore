using System.Collections.Generic;
using Ermes.MapRequests;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class MapRequest_Types_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hazard",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "Layer",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "layers");

            migrationBuilder.AddColumn<List<BoundaryCondition>>(
                name: "BoundaryConditions",
                table: "map_requests",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "map_requests",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DoSpotting",
                table: "map_requests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ProbabilityRange",
                table: "map_requests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TimeLimit",
                table: "map_requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "map_requests",
                nullable: true,
                defaultValue: "FireAndBurnedArea");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "layers",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoundaryConditions",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "DoSpotting",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "ProbabilityRange",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "map_requests");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "layers");

            migrationBuilder.AddColumn<string>(
                name: "Hazard",
                table: "map_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Layer",
                table: "map_requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "layers",
                type: "text",
                nullable: true);
        }
    }
}
