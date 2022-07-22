using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Communication_Scope_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReceiverId",
                table: "communications",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverTeamId",
                table: "communications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "communications",
                nullable: false,
                defaultValue: "Public");

            migrationBuilder.CreateIndex(
                name: "IX_communications_ReceiverId",
                table: "communications",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_communications_ReceiverTeamId",
                table: "communications",
                column: "ReceiverTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_communications_persons_ReceiverId",
                table: "communications",
                column: "ReceiverId",
                principalTable: "persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_communications_teams_ReceiverTeamId",
                table: "communications",
                column: "ReceiverTeamId",
                principalTable: "teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_communications_persons_ReceiverId",
                table: "communications");

            migrationBuilder.DropForeignKey(
                name: "FK_communications_teams_ReceiverTeamId",
                table: "communications");

            migrationBuilder.DropIndex(
                name: "IX_communications_ReceiverId",
                table: "communications");

            migrationBuilder.DropIndex(
                name: "IX_communications_ReceiverTeamId",
                table: "communications");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "communications");

            migrationBuilder.DropColumn(
                name: "ReceiverTeamId",
                table: "communications");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "communications");
        }
    }
}
