using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class File_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "scrape_request");

            migrationBuilder.AddColumn<int>(
                name: "Entries",
                table: "scrape_request",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "scrape_request",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "file",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ContentLength = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    ScrapeRequestId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_scrape_request_ScrapeRequestId",
                        column: x => x.ScrapeRequestId,
                        principalTable: "scrape_request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_file_ScrapeRequestId",
                table: "file",
                column: "ScrapeRequestId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file");

            migrationBuilder.DropColumn(
                name: "Entries",
                table: "scrape_request");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "scrape_request");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "scrape_request",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
