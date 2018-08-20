using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StockLibrary.Migrations
{
    public partial class retry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Retry",
                table: "Fund",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RetryMessage",
                table: "Fund",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Retry",
                table: "Fund");

            migrationBuilder.DropColumn(
                name: "RetryMessage",
                table: "Fund");
        }
    }
}
