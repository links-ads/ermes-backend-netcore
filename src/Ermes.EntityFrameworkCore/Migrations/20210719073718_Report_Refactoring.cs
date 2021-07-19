using System.Collections.Generic;
using Ermes.Reports;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Report_Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "Targets",
                table: "reports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "reports",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<List<ReportTarget>>(
                name: "Targets",
                table: "reports",
                type: "jsonb",
                nullable: true);
        }
    }
}
