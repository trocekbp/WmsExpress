using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Store_Warehouse_App.Migrations
{
    /// <inheritdoc />
    public partial class netandgrosspricesondocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalValue",
                table: "Document",
                newName: "TotalNetAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossPrice",
                table: "DocumentItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetPrice",
                table: "DocumentItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalGrossAmount",
                table: "Document",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossPrice",
                table: "DocumentItem");

            migrationBuilder.DropColumn(
                name: "NetPrice",
                table: "DocumentItem");

            migrationBuilder.DropColumn(
                name: "TotalGrossAmount",
                table: "Document");

            migrationBuilder.RenameColumn(
                name: "TotalNetAmount",
                table: "Document",
                newName: "TotalValue");
        }
    }
}
