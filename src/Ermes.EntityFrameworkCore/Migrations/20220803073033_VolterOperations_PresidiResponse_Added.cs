using Ermes.Operations;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class VolterOperations_PresidiResponse_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<PresidiResponse>(
                name: "PresidiResponse",
                table: "volter_operations",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PresidiResponse",
                table: "volter_operations");
        }
    }
}
