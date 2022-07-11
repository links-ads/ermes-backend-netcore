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
            
            /*
             * This script cannot be executed here, since there the necessity to import the GamificationActions by calling the corresponding API.
             * To add the rewards, once the migrations have been applied, use the Add_Reward.sql script, stored in src/Scripts folder
            migrationBuilder.Sql(
            @"
                    INSERT INTO public.gamification_rewards (""Competence"", ""Name"", ""Discriminator"", ""Detail"", ""GamificationActionCode"", ""Type"", ""Hazard"", ""Points"", ""Description"") 
                        VALUES
                                ('Socializer', 'ShareContentBronze',   'Medal', '{""Threshold"": 3,    ""Points"":  3}', 'SOC.1', 'Bronze', null, null, null), 
                                ('Socializer', 'ShareContentSilver',   'Medal', '{""Threshold"": 30,   ""Points"": 10}', 'SOC.1', 'Silver', null, null, null), 
                                ('Socializer', 'ShareContentGold',     'Medal', '{""Threshold"": 100,  ""Points"": 20}', 'SOC.1', 'Gold', null, null, null), 
                                ('Socializer', 'ShareContentPlatinum', 'Medal', '{""Threshold"": 1000, ""Points"": 50}', 'SOC.1', 'Platinum', null, null, null), 
                                ('Socializer', 'InviteFriendBronze',   'Medal', '{""Threshold"": 3,    ""Points"":  1}', 'SOC.2', 'Bronze', null, null, null), 
                                ('Socializer', 'InviteFriendSilver',   'Medal', '{""Threshold"": 30,   ""Points"":  3}', 'SOC.2', 'Silver', null, null, null), 
                                ('Socializer', 'InviteFriendGold',     'Medal', '{""Threshold"": 100,  ""Points"": 10}', 'SOC.2', 'Gold', null, null, null), 
                                ('Socializer', 'InviteFriendPlatinum', 'Medal', '{""Threshold"": 1000, ""Points"": 20}', 'SOC.2', 'Platinum', null, null, null), 
                                ('Learner', 'ReadTipBronze',   'Medal', '{""Threshold"": 3,   ""Points"":  5}', 'LEA.1', 'Bronze', null, null, null), 
                                ('Learner', 'ReadTipSilver',   'Medal', '{""Threshold"": 30,  ""Points"": 15}', 'LEA.1', 'Silver', null, null, null), 
                                ('Learner', 'ReadTipGold',     'Medal', '{""Threshold"": 100, ""Points"": 30}', 'LEA.1', 'Gold', null, null, null), 
                                ('Learner', 'ReadTipPlatinum', 'Medal', '{""Threshold"": 200, ""Points"": 60}', 'LEA.1', 'Platinum', null, null, null), 
                                ('Learner', 'AnswerQuizBronze',   'Medal', '{""Threshold"": 3,    ""Points"":  10}', 'LEA.2', 'Bronze', null, null, null), 
                                ('Learner', 'AnswerQuizSilver',   'Medal', '{""Threshold"": 30,   ""Points"":  30}', 'LEA.2', 'Silver', null, null, null), 
                                ('Learner', 'AnswerQuizGold',     'Medal', '{""Threshold"": 100,  ""Points"": 100}', 'LEA.2', 'Gold', null, null, null), 
                                ('Learner', 'AnswerQuizPlatinum', 'Medal', '{""Threshold"": 200,  ""Points"": 200}', 'LEA.2', 'Platinum', null, null, null), 
                                ('Reporter', 'DoReportBronze',   'Medal', '{""Threshold"":    3, ""Points"":  15}', 'REP.1', 'Bronze', null, null, null), 
                                ('Reporter', 'DoReportSilver',   'Medal', '{""Threshold"":   30, ""Points"":  45}', 'REP.1', 'Silver', null, null, null), 
                                ('Reporter', 'DoReportGold',     'Medal', '{""Threshold"":  100, ""Points"": 100}', 'REP.1', 'Gold', null, null, null), 
                                ('Reporter', 'DoReportPlatinum', 'Medal', '{""Threshold"": 1000, ""Points"": 300}', 'REP.1', 'Platinum', null, null, null), 
                                ('Learner', 'ReadTipAvalanche', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Avalanche', null, null),
                                ('Learner', 'ReadTipEarthquake', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Earthquake', null, null), 
                                ('Learner', 'ReadTipFire', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Fire', null, null), 
                                ('Learner', 'ReadTipFlood', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Flood', null, null), 
                                ('Learner', 'ReadTipLandslide', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Landslide', null, null), 
                                ('Learner', 'ReadTipStorm', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Storm', null, null), 
                                ('Learner', 'ReadTipWeather', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Weather', null, null), 
                                ('Learner', 'ReadTipSubsidence', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Subsidence', null, null), 
                                ('Learner', 'DoReportAvalanche', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Avalanche', null, null),
                                ('Learner', 'DoReportEarthquake', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Earthquake', null, null), 
                                ('Learner', 'DoReportFire', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Fire', null, null), 
                                ('Learner', 'DoReportFlood', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Flood', null, null), 
                                ('Learner', 'DoReportLandslide', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Landslide', null, null), 
                                ('Learner', 'DoReportStorm', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Storm', null, null), 
                                ('Learner', 'DoReportWeather', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Weather', null, null), 
                                ('Learner', 'DoReportSubsidence', 'Badge', '{""Threshold"":100, ""Points"": 200}', 'REP.1', null, 'Subsidence', null, null), 
                                ('Reporter', 'TopReporter', 'Award', null, null, null, null, 25, 'highest score obtained in the Reported category, overall'), 
                                ('Reporter', 'BestReport', 'Award', null, null, null, null, 15, 'owner of the most upvoted report');                             
                ");*/
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
