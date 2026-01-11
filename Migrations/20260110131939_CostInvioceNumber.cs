using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Store_Warehouse_App.Migrations
{
    /// <inheritdoc />
    public partial class CostInvioceNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CostInvoiceNumber",
                table: "Document",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostInvoiceNumber",
                table: "Document");
        }
    }
}
