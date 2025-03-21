using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class propstest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrandName",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "BuyDate",
                table: "IT.Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Deprecated",
                table: "IT.Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DownDate",
                table: "IT.Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GetDate",
                table: "IT.Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MinYears",
                table: "IT.Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "IT.Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrandName",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "BuyDate",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "Deprecated",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "DownDate",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "GetDate",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "MinYears",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IT.Properties");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "IT.Properties");
        }
    }
}
