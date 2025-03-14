using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class itemreportid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "IT.Items");

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "IT.ItemBalanceSheets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "IT.Items",
                type: "int",
                nullable: true);
        }
    }
}
