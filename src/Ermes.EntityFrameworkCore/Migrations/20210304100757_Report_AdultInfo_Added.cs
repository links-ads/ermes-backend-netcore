using System.Collections.Generic;
using Ermes.Reports;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Report_AdultInfo_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<ReportAdultInfo>>(
                name: "AdultInfo",
                table: "reports",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdultInfo",
                table: "reports");
        }
    }
}
