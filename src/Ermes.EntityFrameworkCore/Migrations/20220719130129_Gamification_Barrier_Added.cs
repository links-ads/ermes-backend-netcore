using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Gamification_Barrier_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "gamification_levels",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_gamification_rewards_Name",
                table: "gamification_rewards",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_gamification_levels_Name",
                table: "gamification_levels",
                column: "Name");

            migrationBuilder.CreateTable(
                name: "gamification_barriers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LevelName = table.Column<string>(nullable: false),
                    RewardName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_barriers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gamification_barriers_gamification_levels_LevelName",
                        column: x => x.LevelName,
                        principalTable: "gamification_levels",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gamification_barriers_gamification_rewards_RewardName",
                        column: x => x.RewardName,
                        principalTable: "gamification_rewards",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_gamification_barriers_RewardName",
                table: "gamification_barriers",
                column: "RewardName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gamification_barriers_LevelName_RewardName",
                table: "gamification_barriers",
                columns: new[] { "LevelName", "RewardName" },
                unique: true);

            /*
             * This script cannot be executed here, since there the necessity to import the Rewards.
             * To add the barriers, once the migrations have been applied, use the Add_Barriers.sql script, stored in src/Scripts folder
            migrationBuilder.Sql(
                @"
                    INSERT INTO public.gamification_barriers (""LevelName"", ""RewardName"") 
                        VALUES
                            ('Watcher',  'AnswerQuizBronze'),
                            ('Watcher',  'ReadTipSilver'),
                            ('Sentinel', 'AnswerQuizSilver'),
                            ('Sentinel', 'DoReportBronze'),
                            ('Explorer', 'ReadTipGold'),
                            ('Explorer', 'DoReportSilver'),
                            ('Master',   'AnswerQuizGold'),
                            ('Master',   'DoReportGold'),
                            ('Guru',     'ReadTipPlatinum'),
                            ('Guru',     'DoReportPlatinum')
                ");
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gamification_barriers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_gamification_rewards_Name",
                table: "gamification_rewards");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_gamification_levels_Name",
                table: "gamification_levels");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "gamification_levels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
