using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Organization_Parent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "organizations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_organizations_ParentId",
                table: "organizations",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_organizations_organizations_ParentId",
                table: "organizations",
                column: "ParentId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_organizations_organizations_ParentId",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_ParentId",
                table: "organizations");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "organizations");
        }
    }
}
