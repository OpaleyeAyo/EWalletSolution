using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EWallet.DataLayer.Migrations
{
    public partial class currencyLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfInterestAddition",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfInterestAddition",
                table: "Wallets");
        }
    }
}
