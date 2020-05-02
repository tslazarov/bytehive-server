using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class RequestFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "scrape_request");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "scrape_request",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "scrape_request");

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "scrape_request",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
