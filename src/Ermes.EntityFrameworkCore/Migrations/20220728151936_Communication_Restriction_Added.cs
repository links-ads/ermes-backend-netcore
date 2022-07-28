using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Communication_Restriction_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Restriction",
                table: "communications",
                nullable: false,
                defaultValue: "None");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Restriction",
                table: "communications");

            migrationBuilder.AddColumn<long>(
                name: "ReceiverId",
                table: "communications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiverTeamId",
                table: "communications",
                type: "integer",
                nullable: true);

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
    }
}
