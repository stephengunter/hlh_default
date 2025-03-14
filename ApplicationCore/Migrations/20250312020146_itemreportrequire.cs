using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class itemreportrequire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "IT.ItemBalanceSheets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IT.ItemBalanceSheets_ItemId",
                table: "IT.ItemBalanceSheets",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_IT.ItemBalanceSheets_ReportId",
                table: "IT.ItemBalanceSheets",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_IT.ItemBalanceSheets_IT.ItemReports_ReportId",
                table: "IT.ItemBalanceSheets",
                column: "ReportId",
                principalTable: "IT.ItemReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IT.ItemBalanceSheets_IT.Items_ItemId",
                table: "IT.ItemBalanceSheets",
                column: "ItemId",
                principalTable: "IT.Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IT.ItemBalanceSheets_IT.ItemReports_ReportId",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropForeignKey(
                name: "FK_IT.ItemBalanceSheets_IT.Items_ItemId",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropIndex(
                name: "IX_IT.ItemBalanceSheets_ItemId",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.DropIndex(
                name: "IX_IT.ItemBalanceSheets_ReportId",
                table: "IT.ItemBalanceSheets");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "IT.ItemBalanceSheets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
