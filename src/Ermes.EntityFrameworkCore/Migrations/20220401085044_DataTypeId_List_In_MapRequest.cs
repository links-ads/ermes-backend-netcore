using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class DataTypeId_List_In_MapRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE map_requests DROP COLUMN \"DataTypeId\"");

            migrationBuilder.AddColumn<List<int>>(
                name: "DataTypeIds",
                table: "map_requests",
                nullable: true,
                defaultValue: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DataTypeId",
                table: "map_requests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(List<int>),
                oldNullable: true);
        }
    }
}
