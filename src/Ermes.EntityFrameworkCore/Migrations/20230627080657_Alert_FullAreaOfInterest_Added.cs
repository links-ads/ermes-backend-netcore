using Ermes.Alerts;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ermes.Migrations
{
    public partial class Alert_FullAreaOfInterest_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Geometry>(
                name: "FullAreaOfInterest",
                table: "alerts",
                type: "geography",
                nullable: true);

            migrationBuilder.Sql(@"update public.alerts set ""FullAreaOfInterest"" = ""AreaOfInterest""");
            migrationBuilder.Sql(@"update public.alerts set ""AreaOfInterest"" = ST_Envelope(""FullAreaOfInterest""::geometry)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullAreaOfInterest",
                table: "alerts");
        }
    }
}
