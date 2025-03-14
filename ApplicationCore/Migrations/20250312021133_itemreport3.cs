using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class itemreport3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IT.ItemBalanceSheets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "IT.ItemBalanceSheets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "IT.ItemBalanceSheets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "IT.ItemBalanceSheets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IT.ItemBalanceSheets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
