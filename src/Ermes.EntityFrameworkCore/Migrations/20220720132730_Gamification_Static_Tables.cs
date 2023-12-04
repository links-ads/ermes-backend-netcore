using System;
using Ermes.Gamification;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Gamification_Static_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "tip_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "quiz_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "persons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "persons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "layer_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "category_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "answer_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "activity_translations",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "gamification_actions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Points = table.Column<int>(nullable: false),
                    Competence = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_actions", x => x.Id);
                    table.UniqueConstraint("AK_gamification_actions_Code", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "gamification_levels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    LowerBound = table.Column<int>(nullable: false),
                    UpperBound = table.Column<int>(nullable: false),
                    PreviousLevelId = table.Column<int>(nullable: true),
                    FollowingLevelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_levels", x => x.Id);
                    table.UniqueConstraint("AK_gamification_levels_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_gamification_levels_gamification_levels_FollowingLevelId",
                        column: x => x.FollowingLevelId,
                        principalTable: "gamification_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_gamification_levels_gamification_levels_PreviousLevelId",
                        column: x => x.PreviousLevelId,
                        principalTable: "gamification_levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    CrisisPhase = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Points = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_rewards", x => x.Id);
                    table.UniqueConstraint("AK_gamification_rewards_Name", x => x.Name);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gamification_audit_gamification_rewards_RewardId",
                        column: x => x.RewardId,
                        principalTable: "gamification_rewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_persons_LevelId",
                table: "persons",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_actions_Code",
                table: "gamification_actions",
                column: "Code",
                unique: true);

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
                name: "IX_gamification_barriers_RewardName",
                table: "gamification_barriers",
                column: "RewardName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gamification_barriers_LevelName_RewardName",
                table: "gamification_barriers",
                columns: new[] { "LevelName", "RewardName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gamification_levels_FollowingLevelId",
                table: "gamification_levels",
                column: "FollowingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_levels_PreviousLevelId",
                table: "gamification_levels",
                column: "PreviousLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_rewards_GamificationActionCode",
                table: "gamification_rewards",
                column: "GamificationActionCode");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_gamification_levels_LevelId",
                table: "persons",
                column: "LevelId",
                principalTable: "gamification_levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            #region Levels
            migrationBuilder.Sql(
                @"
                    INSERT INTO public.gamification_levels (""Name"", ""LowerBound"", ""UpperBound"", ""PreviousLevelId"", ""FollowingLevelId"") 
                    VALUES
                        ('Rookie', 0, 150, null, null), 
                        ('Novice', 151, 305, null, null), 
                        ('Junior', 306, 813, null, null), 
                        ('Expert', 814, 2488, null, null), 
                        ('Master', 2489, 4968, null, null), 
                        ('Guru', 4969, 1000000, null, null);
                ");

            migrationBuilder.Sql(
                @"
                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Novice') where ""Name"" = 'Rookie';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Junior') where ""Name"" = 'Novice';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Rookie') where ""Name"" = 'Novice';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Expert') where ""Name"" = 'Junior';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Novice') where ""Name"" = 'Junior';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Master') where ""Name"" = 'Expert';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Junior') where ""Name"" = 'Expert';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Guru') where ""Name"" = 'Master';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Expert') where ""Name"" = 'Master';

                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Master') where ""Name"" = 'Guru';
                "
            );

            //Assign default Rookie level to every citizen
            migrationBuilder.Sql(
                @"
                    update public.persons p2
                    set ""LevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Rookie')
                    where p2.""Id"" in (
                        select p.""Id""
                        from persons p
                        join person_roles pr on pr.""PersonId"" = p.""Id""
                        join roles r on r.""Id"" = pr.""RoleId""
                        where r.""Name"" = 'citizen'
                    )
                ");
            #endregion

            #region GamificationActions
            migrationBuilder.Sql(
                @"
                    INSERT INTO public.gamification_actions (""Code"", ""Competence"", ""Name"", ""Points"", ""Description"") 
                        VALUES
                            ('ONB.1', 'Onboarding', 'FirstLogin', 10, 'First Login'),
                            ('ONB.2', 'Onboarding', 'CompleteWizard', 100, 'Complete wizard'),

                            ('LEA.1', 'Learning', 'ReadTip', 3, 'Read tip'),
                            ('LEA.2', 'Learning', 'AnswerQuiz', 5, 'Answer quiz'),
                            ('LEA.3', 'Learning', 'AllTipsRead', 100, 'All tips have been read'),
                            ('LEA.4', 'Learning', 'AllQuizzesAnswered', 100, 'All quizzes have been solved'),

                            ('REP.1', 'Reporting', 'DoReport', 5, 'Do a report'),
                            ('REP.2', 'Reporting', 'FirstReport', 10, 'This is the first report created by the user'),

                            ('REW.1', 'Reviewing', 'ValidateReport', 1, 'Validate a report'),
                            ('REW.2', 'Reviewing', 'ReportGetValidatedByPeer', 1, 'Report has been validated by a peer'),
                            ('REW.3', 'Reviewing', 'ReportGetRejectedByPeer', -1, 'Report has been rejected by a peer'),
                            ('REW.4', 'Reviewing', 'ReportGetValidatedByPro', 5, 'Report has been validated by a professional'),
                            ('REW.5', 'Reviewing', 'ReportGetRejectedByPro', -5, 'Report has been rejected by a professioanl')
                ");
            #endregion

            #region Rewards
            migrationBuilder.Sql(
                @"
                    INSERT INTO public.gamification_rewards (""Competence"", ""Name"", ""Discriminator"", ""Detail"", ""GamificationActionCode"", ""Type"", ""CrisisPhase"", ""Points"", ""Description"") 
                        VALUES
                                ('Learning', 'ReadTipBronze',   'Medal', '{""Threshold"": 10,   ""Points"": 30}', 'LEA.1', 'Bronze', null, null, null), 
                                ('Learning', 'ReadTipSilver',   'Medal', '{""Threshold"": 30,  ""Points"": 150}', 'LEA.1', 'Silver', null, null, null), 
                                ('Learning', 'ReadTipGold',     'Medal', '{""Threshold"": 100, ""Points"": 300}', 'LEA.1', 'Gold', null, null, null), 
                                ('Learning', 'ReadTipPlatinum', 'Medal', '{""Threshold"": 200, ""Points"": 600}', 'LEA.1', 'Platinum', null, null, null), 
                                ('Learning', 'AnswerQuizBronze',   'Medal', '{""Threshold"": 10,    ""Points"": 50}', 'LEA.2', 'Bronze', null, null, null), 
                                ('Learning', 'AnswerQuizSilver',   'Medal', '{""Threshold"": 30,   ""Points"":  150}', 'LEA.2', 'Silver', null, null, null), 
                                ('Learning', 'AnswerQuizGold',     'Medal', '{""Threshold"": 100,  ""Points"": 500}', 'LEA.2', 'Gold', null, null, null), 
                                ('Learning', 'AnswerQuizPlatinum', 'Medal', '{""Threshold"": 200,  ""Points"": 1000}', 'LEA.2', 'Platinum', null, null, null), 
                                ('Reporting', 'DoReportBronze',   'Medal', '{""Threshold"":    3, ""Points"":  30}', 'REP.1', 'Bronze', null, null, null), 
                                ('Reporting', 'DoReportSilver',   'Medal', '{""Threshold"":   10, ""Points"":  100}', 'REP.1', 'Silver', null, null, null), 
                                ('Reporting', 'DoReportGold',     'Medal', '{""Threshold"":  50, ""Points"": 500}', 'REP.1', 'Gold', null, null, null), 
                                ('Reporting', 'DoReportPlatinum', 'Medal', '{""Threshold"": 100, ""Points"": 1000}', 'REP.1', 'Platinum', null, null, null), 
                                ('Reviewing', 'ReviewReportBronze',   'Medal', '{""Threshold"": 3,    ""Points"": 3}', 'REW.1', 'Bronze', null, null, null), 
                                ('Reviewing', 'ReviewReportSilver',   'Medal', '{""Threshold"": 30,   ""Points"": 30}', 'REW.1', 'Silver', null, null, null), 
                                ('Reviewing', 'ReviewReportGold',     'Medal', '{""Threshold"": 100,  ""Points"": 100}', 'REW.1', 'Gold', null, null, null), 
                                ('Reviewing', 'ReviewReportPlatinum', 'Medal', '{""Threshold"": 300, ""Points"": 300}', 'REW.1', 'Platinum', null, null, null), 
                                ('Learning', 'ReadTipPrevention', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Prevention', null, null),
                                ('Learning', 'ReadTipPreparedness', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Preparedness', null, null), 
                                ('Learning', 'ReadTipResponse', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Response', null, null), 
                                ('Learning', 'ReadTipPostEvent', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'PostEvent', null, null), 
                                ('Learning', 'ReadTipEnvironment', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.1', null, 'Environment', null, null), 
                                ('Learning', 'AnswerQuizPrevention', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.2', null, 'Prevention', null, null),
                                ('Learning', 'AnswerQuizPreparedness', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.2', null, 'Preparedness', null, null), 
                                ('Learning', 'AnswerQuizResponse', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.2', null, 'Response', null, null), 
                                ('Learning', 'AnswerQuizPostEvent', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.2', null, 'PostEvent', null, null), 
                                ('Learning', 'AnswerQuizEnvironment', 'Badge', '{""Threshold"": -1, ""Points"": 100}', 'LEA.2', null, 'Environment', null, null), 
                                ('Learning', 'BestLearnerTip', 'Award', null, null, null, null, 15, 'User reading more tips in a week'), 
                                ('Learning', 'BestLearnerQuiz', 'Award', null, null, null, null, 15, 'User replying more quizzes in a week'), 
                                ('Reporting', 'TopReporter', 'Award', null, null, null, null, 15, 'User sending the highest number of reports in a week'),
                                ('Reporting', 'BestReporter', 'Award', null, null, null, null, 30, 'User with the highest total points from reviews in a week'), 
                                ('Reviewing', 'TopReviewer', 'Award', null, null, null, null, 15, 'User providing the highest number of reviews in a week')                                                           
                ");
            #endregion

            #region Barriers
            migrationBuilder.Sql(
                @"
                    INSERT INTO public.gamification_barriers (""LevelName"", ""RewardName"") 
                        VALUES
                            ('Novice', 'AnswerQuizBronze'),
                            ('Novice', 'ReadTipSilver'),
                            ('Novice', 'DoReportBronze'),

                            ('Junior', 'AnswerQuizSilver'),
                            ('Junior', 'ReadTipGold'),
                            ('Junior', 'DoReportSilver'),
                            ('Junior', 'ReviewReportBronze'),

                            ('Expert', 'ReadTipPlatinum'),
                            ('Expert', 'AnswerQuizGold'),
                            ('Expert', 'DoReportGold'),
                            ('Expert', 'ReviewReportSilver'),
                            
                            ('Master', 'AnswerQuizPlatinum'),
                            ('Master', 'DoReportPlatinum'),
                            ('Master', 'ReviewReportGold'),

                            ('Guru', 'ReviewReportPlatinum')
                ");
            #endregion
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_gamification_levels_LevelId",
                table: "persons");

            migrationBuilder.DropTable(
                name: "gamification_audit");

            migrationBuilder.DropTable(
                name: "gamification_barriers");

            migrationBuilder.DropTable(
                name: "gamification_levels");

            migrationBuilder.DropTable(
                name: "gamification_rewards");

            migrationBuilder.DropTable(
                name: "gamification_actions");

            migrationBuilder.DropIndex(
                name: "IX_persons_LevelId",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "persons");

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "tip_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "quiz_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "layer_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "category_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "answer_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "activity_translations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2,
                oldNullable: true);
        }
    }
}
