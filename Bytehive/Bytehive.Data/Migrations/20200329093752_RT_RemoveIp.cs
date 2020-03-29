using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class RT_RemoveIp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expires",
                table: "refresh_token");

            migrationBuilder.DropColumn(
                name: "RemoteIpAddress",
                table: "refresh_token");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "refresh_token",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "refresh_token");

            migrationBuilder.AddColumn<DateTime>(
                name: "Expires",
                table: "refresh_token",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RemoteIpAddress",
                table: "refresh_token",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
