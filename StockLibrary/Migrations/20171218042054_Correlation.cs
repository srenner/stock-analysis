using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace StockLibrary.Migrations
{
    public partial class Correlation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorrelatedIncrease",
                columns: table => new
                {
                    CorrelatedIncreaseID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrimaryFundDayID = table.Column<int>(nullable: false),
                    SecondaryFundDayID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrelatedIncrease", x => x.CorrelatedIncreaseID);
                    table.ForeignKey(
                        name: "FK_CorrelatedIncrease_FundDay_PrimaryFundDayID",
                        column: x => x.PrimaryFundDayID,
                        principalTable: "FundDay",
                        principalColumn: "FundDayID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorrelatedIncrease_FundDay_SecondaryFundDayID",
                        column: x => x.SecondaryFundDayID,
                        principalTable: "FundDay",
                        principalColumn: "FundDayID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorrelatedIncrease_PrimaryFundDayID",
                table: "CorrelatedIncrease",
                column: "PrimaryFundDayID");

            migrationBuilder.CreateIndex(
                name: "IX_CorrelatedIncrease_SecondaryFundDayID",
                table: "CorrelatedIncrease",
                column: "SecondaryFundDayID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrelatedIncrease");
        }
    }
}
