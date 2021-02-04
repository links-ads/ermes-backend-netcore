using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class PersonAction_CurrentActivity_CurrentExtensionData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentActivityId",
                table: "person_actions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentExtensionData",
                table: "person_actions",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_person_actions_CurrentActivityId",
                table: "person_actions",
                column: "CurrentActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_person_actions_activities_CurrentActivityId",
                table: "person_actions",
                column: "CurrentActivityId",
                principalTable: "activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_person_actions_activities_CurrentActivityId",
                table: "person_actions");

            migrationBuilder.DropIndex(
                name: "IX_person_actions_CurrentActivityId",
                table: "person_actions");

            migrationBuilder.DropColumn(
                name: "CurrentActivityId",
                table: "person_actions");

            migrationBuilder.DropColumn(
                name: "CurrentExtensionData",
                table: "person_actions");
        }
    }
}
