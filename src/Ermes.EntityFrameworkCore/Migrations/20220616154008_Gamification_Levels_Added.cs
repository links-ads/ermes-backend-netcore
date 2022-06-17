using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Ermes.Migrations
{
    public partial class Gamification_Levels_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "persons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "persons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "gamification_levels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    LowerBound = table.Column<int>(nullable: false),
                    UpperBound = table.Column<int>(nullable: false),
                    PreviousLevelId = table.Column<int>(nullable: true),
                    FollowingLevelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gamification_levels", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_persons_LevelId",
                table: "persons",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_levels_FollowingLevelId",
                table: "gamification_levels",
                column: "FollowingLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_gamification_levels_PreviousLevelId",
                table: "gamification_levels",
                column: "PreviousLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_persons_gamification_levels_LevelId",
                table: "persons",
                column: "LevelId",
                principalTable: "gamification_levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("INSERT INTO public.gamification_levels (\"Name\", \"LowerBound\", \"UpperBound\", \"PreviousLevelId\", \"FollowingLevelId\") VALUES('Rookie', 0, 69, null, null), ('Watcher', 70, 299, null, null), ('Sentinel', 300, 799, null, null), ('Explorer', 800, 1999, null, null), ('Master', 2000, 11999, null, null), ('Guru', 12000, 100000, null, null);");

            migrationBuilder.Sql(
                @"
                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Watcher') where ""Name"" = 'Rookie';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Sentinel') where ""Name"" = 'Watcher';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Rookie') where ""Name"" = 'Watcher';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Explorer') where ""Name"" = 'Sentinel';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Watcher') where ""Name"" = 'Sentinel';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Master') where ""Name"" = 'Explorer';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Sentinel') where ""Name"" = 'Explorer';

                    update public.gamification_levels set ""FollowingLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Guru') where ""Name"" = 'Master';
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Explorer') where ""Name"" = 'Master';
                    
                    update public.gamification_levels set ""PreviousLevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Master') where ""Name"" = 'Guru';
                "
            );

            migrationBuilder.Sql(
                @"
                    update public.persons set ""LevelId"" = (select ""Id"" from public.gamification_levels where ""Name"" = 'Rookie');
                "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_persons_gamification_levels_LevelId",
                table: "persons");

            migrationBuilder.DropTable(
                name: "gamification_levels");

            migrationBuilder.DropIndex(
                name: "IX_persons_LevelId",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "persons");
        }
    }
}
