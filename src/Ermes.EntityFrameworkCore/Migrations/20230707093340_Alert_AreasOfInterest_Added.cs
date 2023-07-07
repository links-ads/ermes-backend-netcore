using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Alert_AreasOfInterest_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alert_areasofinterest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertId = table.Column<int>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_areasofinterest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_areasofinterest_alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
            });

            migrationBuilder.Sql(@"
                    insert into public.alert_areasofinterest (""AlertId"", ""AreaOfInterest"", ""CreationTime"")
                    select ""Id"", ""FullAreaOfInterest"", ""CreationTime""
                    from public.alerts
                ");

            migrationBuilder.DropColumn(
                name: "FullAreaOfInterest",
                table: "alerts");

            migrationBuilder.RenameColumn(
                name: "AreaOfInterest",
                table: "alerts",
                newName: "BoundingBox");

            migrationBuilder.AddColumn<int>(
                name: "AlertAreaOfInterestId",
                table: "alerts",
                nullable: true);

            migrationBuilder.Sql(@"
                    update public.alerts a
                    set ""AlertAreaOfInterestId"" =
                    (
                        select ""Id""
                        from public.alert_areasofinterest
                        where a.""Id"" = ""AlertId""
                    ) 
                ");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_AlertAreaOfInterestId",
                table: "alerts",
                column: "AlertAreaOfInterestId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_areasofinterest_AlertId",
                table: "alert_areasofinterest",
                column: "AlertId");

            migrationBuilder.AddForeignKey(
                name: "FK_alerts_alert_areasofinterest_AlertAreaOfInterestId",
                table: "alerts",
                column: "AlertAreaOfInterestId",
                principalTable: "alert_areasofinterest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alerts_alert_areasofinterest_AlertAreaOfInterestId",
                table: "alerts");

            migrationBuilder.DropTable(
                name: "alert_areasofinterest");

            migrationBuilder.DropIndex(
                name: "IX_alerts_AlertAreaOfInterestId",
                table: "alerts");

            migrationBuilder.DropColumn(
                name: "AlertAreaOfInterestId",
                table: "alerts");

            migrationBuilder.RenameColumn(
                name: "BoundingBox",
                table: "alerts",
                newName: "AreaOfInterest");

            migrationBuilder.AddColumn<Geometry>(
                name: "FullAreaOfInterest",
                table: "alerts",
                type: "geography",
                nullable: true);
        }
    }
}
