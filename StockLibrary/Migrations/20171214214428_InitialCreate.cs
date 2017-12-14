using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StockLibrary.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fund",
                columns: table => new
                {
                    Symbol = table.Column<string>(nullable: false),
                    Exchange = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fund", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "FundDay",
                columns: table => new
                {
                    FundDayID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Close = table.Column<decimal>(nullable: false),
                    FundDayDate = table.Column<DateTime>(nullable: false),
                    High = table.Column<decimal>(nullable: false),
                    Low = table.Column<decimal>(nullable: false),
                    Open = table.Column<decimal>(nullable: false),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundDay", x => x.FundDayID);
                    table.ForeignKey(
                        name: "FK_FundDay_Fund_Symbol",
                        column: x => x.Symbol,
                        principalTable: "Fund",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundDay_Symbol",
                table: "FundDay",
                column: "Symbol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundDay");

            migrationBuilder.DropTable(
                name: "Fund");
        }
    }
}
