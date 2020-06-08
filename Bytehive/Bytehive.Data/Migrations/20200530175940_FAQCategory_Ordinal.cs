using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class FAQCategory_Ordinal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Ordinal",
                table: "faq_category",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ordinal",
                table: "faq_category");
        }
    }
}
