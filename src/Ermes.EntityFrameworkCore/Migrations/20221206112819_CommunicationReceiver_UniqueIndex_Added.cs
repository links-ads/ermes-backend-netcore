using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class CommunicationReceiver_UniqueIndex_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_communication_receivers_CommunicationId",
                table: "communication_receivers");

            migrationBuilder.CreateIndex(
                name: "IX_communication_receivers_CommunicationId_OrganizationId",
                table: "communication_receivers",
                columns: new[] { "CommunicationId", "OrganizationId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_communication_receivers_CommunicationId_OrganizationId",
                table: "communication_receivers");

            migrationBuilder.CreateIndex(
                name: "IX_communication_receivers_CommunicationId",
                table: "communication_receivers",
                column: "CommunicationId");
        }
    }
}
