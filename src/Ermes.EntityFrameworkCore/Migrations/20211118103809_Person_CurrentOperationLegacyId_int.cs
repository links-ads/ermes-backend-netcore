using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Person_CurrentOperationLegacyId_int : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql("ALTER TABLE persons ALTER COLUMN \"CurrentOperationLegacyId\" TYPE integer USING \"CurrentOperationLegacyId\"::integer");

            /*
             * Automatically generated code for this migration is not working
             */
        //    migrationBuilder.AlterColumn<int>(
        //        name: "CurrentOperationLegacyId",
        //        table: "persons",
        //        nullable: true,
        //        oldClrType: typeof(string),
        //        oldType: "text",
        //        oldNullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrentOperationLegacyId",
                table: "persons",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
