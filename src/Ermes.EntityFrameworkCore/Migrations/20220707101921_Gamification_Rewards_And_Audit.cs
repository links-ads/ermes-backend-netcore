using System;
using Ermes.Gamification;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Gamification_Rewards_And_Audit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_gamification_actions_Code",
                table: "gamification_actions",
                column: "Code");

            migrationBuilder.CreateTable(
                name: "gamification_rewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Competence = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 1000, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Detail = table.Column<GamificationDetail>(type: "jsonb", nullable: true),
                    GamificationActionCode = table.Column<string>(nullable: true),
                    Hazard = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_rewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gamification_rewards_gamification_actions_GamificationActio~",
                        column: x => x.GamificationActionCode,
                        principalTable: "gamification_actions",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gamification_audit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<long>(nullable: false),
                    RewardId = table.Column<int>(nullable: true),
                    GamificationActionId = table.Column<int>(nullable: true),
                    LevelId = table.Column<int>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_audit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gamification_audit_gamification_actions_GamificationActionId",
                        column: x => x.GamificationActionId,
                        principalTable: "gamification_actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gamification_audit_gamification_levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "gamification_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gamification_audit_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gamification_audit_gamification_rewards_RewardId",
                        column: x => x.RewardId,
                        principalTable: "gamification_rewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gamification_audit_GamificationActionId",
                table: "gamification_audit",
                column: "GamificationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_audit_LevelId",
                table: "gamification_audit",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_audit_PersonId",
                table: "gamification_audit",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_audit_RewardId",
                table: "gamification_audit",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_rewards_GamificationActionCode",
                table: "gamification_rewards",
                column: "GamificationActionCode");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gamification_audit");

            migrationBuilder.DropTable(
                name: "gamification_rewards");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_gamification_actions_Code",
                table: "gamification_actions");
        }
    }
}
