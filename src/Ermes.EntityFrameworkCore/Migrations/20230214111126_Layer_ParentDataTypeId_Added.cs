using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Layer_ParentDataTypeId_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentDataTypeId",
                table: "layers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_layers_ParentDataTypeId",
                table: "layers",
                column: "ParentDataTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_layers_layers_ParentDataTypeId",
                table: "layers",
                column: "ParentDataTypeId",
                principalTable: "layers",
                principalColumn: "DataTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_layers_layers_ParentDataTypeId",
                table: "layers");

            migrationBuilder.DropIndex(
                name: "IX_layers_ParentDataTypeId",
                table: "layers");

            migrationBuilder.DropColumn(
                name: "ParentDataTypeId",
                table: "layers");
        }
    }
}
