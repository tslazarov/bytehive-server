using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class Payment_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tokens",
                table: "user",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "payment_tier",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false),
                    Sku = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_tier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ExternalId = table.Column<string>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    PaymentTierId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment_payment_tier_PaymentTierId",
                        column: x => x.PaymentTierId,
                        principalTable: "payment_tier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payment_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment_PaymentTierId",
                table: "payment",
                column: "PaymentTierId");

            migrationBuilder.CreateIndex(
                name: "IX_payment_UserId",
                table: "payment",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "payment_tier");

            migrationBuilder.DropColumn(
                name: "Tokens",
                table: "user");
        }
    }
}
