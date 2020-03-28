using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class User_Provider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "user");

            migrationBuilder.DropColumn(
                name: "UserExternalId",
                table: "user");

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "user",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "user");

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "user",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserExternalId",
                table: "user",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
