using Microsoft.EntityFrameworkCore.Migrations;

namespace Ermes.Migrations
{
    public partial class Person_Email_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "persons",
                maxLength: 255,
                nullable: true);

            migrationBuilder.Sql(
                @"
                    update public.persons p2
                    set ""Email"" = concat(""Username"", '@europe.com')
                    where ""Username"" is not null
                "
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "persons");
        }
    }
}
