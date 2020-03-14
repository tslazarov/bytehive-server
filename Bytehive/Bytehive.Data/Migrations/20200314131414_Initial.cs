using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "faq_category",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NameEN = table.Column<string>(nullable: true),
                    NameBG = table.Column<string>(nullable: true),
                    MyProperty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 40, nullable: false),
                    FirstName = table.Column<string>(maxLength: 30, nullable: true),
                    LastName = table.Column<string>(maxLength: 30, nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    HashedPassword = table.Column<string>(nullable: true),
                    DefaultLanguage = table.Column<int>(nullable: false),
                    ResetCode = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    IsExternal = table.Column<bool>(nullable: false),
                    UserExternalId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "faq",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuestionEN = table.Column<string>(nullable: true),
                    QuestionBG = table.Column<string>(nullable: true),
                    AnswerEN = table.Column<string>(nullable: true),
                    AnswerBG = table.Column<string>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FAQs_FAQCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "faq_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "scrape_request",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ScrapeType = table.Column<int>(nullable: false),
                    ExportType = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: false),
                    ValidationKey = table.Column<string>(nullable: true),
                    DownloadUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrapeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScrapeRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_CategoryId",
                table: "faq",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrapeRequests_UserId",
                table: "scrape_request",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "user",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "faq");

            migrationBuilder.DropTable(
                name: "scrape_request");

            migrationBuilder.DropTable(
                name: "faq_category");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
