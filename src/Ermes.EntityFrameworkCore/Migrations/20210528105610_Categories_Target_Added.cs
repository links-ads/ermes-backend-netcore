using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Categories_Target_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "category_translations");

            migrationBuilder.DropColumn(
                name: "StatusValues",
                table: "categories");

            migrationBuilder.AlterColumn<string>(
                name: "SubGroup",
                table: "category_translations",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "GroupCode",
                table: "categories",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "categories",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "categories",
                nullable: false,
                defaultValue: "None");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Target",
                table: "categories");

            migrationBuilder.AlterColumn<string>(
                name: "SubGroup",
                table: "category_translations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "category_translations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GroupCode",
                table: "categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string[]>(
                name: "StatusValues",
                table: "categories",
                type: "text[]",
                nullable: true);
        }
    }
}
