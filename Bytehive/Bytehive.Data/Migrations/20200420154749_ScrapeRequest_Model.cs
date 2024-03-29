﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bytehive.Data.Migrations
{
    public partial class ScrapeRequest_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "scrape_request",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "scrape_request",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "scrape_request");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "scrape_request");
        }
    }
}
