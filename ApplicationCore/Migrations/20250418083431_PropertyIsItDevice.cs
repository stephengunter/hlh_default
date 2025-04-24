using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    /// <inheritdoc />
    public partial class PropertyIsItDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsItDevice",
                table: "IT.Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsItDevice",
                table: "IT.Properties");
        }
    }
}
