using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class ScrapeRequest_AccessKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidationKey",
                table: "scrape_request");

            migrationBuilder.AddColumn<string>(
                name: "AccessKey",
                table: "scrape_request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessKey",
                table: "scrape_request");

            migrationBuilder.AddColumn<string>(
                name: "ValidationKey",
                table: "scrape_request",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
