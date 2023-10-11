using System;
using System.Collections.Generic;
using Ermes.Alerts;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Alerts_And_NotificationReceived_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identifier = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Sent = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    MsgType = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    References = table.Column<string>(nullable: true),
                    Restriction = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    AreaId = table.Column<string>(nullable: true),
                    IsARecommendation = table.Column<bool>(nullable: false),
                    AreaOfInterest = table.Column<Geometry>(type: "geography", nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "notifications_received",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoutingKey = table.Column<string>(maxLength: 255, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications_received", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "alerts_cap_info",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AlertId = table.Column<int>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Event = table.Column<string>(nullable: true),
                    ResponseType = table.Column<string>(nullable: true),
                    Urgency = table.Column<string>(nullable: true),
                    Severity = table.Column<string>(nullable: true),
                    Certainty = table.Column<string>(nullable: true),
                    Expires = table.Column<DateTime>(nullable: false),
                    Headline = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Area = table.Column<List<CapArea>>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts_cap_info", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alerts_cap_info_alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_cap_info_AlertId",
                table: "alerts_cap_info",
                column: "AlertId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts_cap_info");

            migrationBuilder.DropTable(
                name: "notifications_received");

            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
