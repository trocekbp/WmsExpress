using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Store_Warehouse_App.Migrations
{
    /// <inheritdoc />
    public partial class categoryattributesrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Category_CategoryId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_AtrDefinition_AtrGroup_AttributeGroupId",
                table: "AtrDefinition");

            migrationBuilder.DropTable(
                name: "AtrGroup");

            migrationBuilder.RenameColumn(
                name: "AttributeGroupId",
                table: "AtrDefinition",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_AtrDefinition_AttributeGroupId",
                table: "AtrDefinition",
                newName: "IX_AtrDefinition_CategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Attribute",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Category_CategoryId",
                table: "Article",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AtrDefinition_Category_CategoryId",
                table: "AtrDefinition",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Category_CategoryId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_AtrDefinition_Category_CategoryId",
                table: "AtrDefinition");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "AtrDefinition",
                newName: "AttributeGroupId");

            migrationBuilder.RenameIndex(
                name: "IX_AtrDefinition_CategoryId",
                table: "AtrDefinition",
                newName: "IX_AtrDefinition_AttributeGroupId");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Attribute",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AtrGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtrGroup", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Category_CategoryId",
                table: "Article",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AtrDefinition_AtrGroup_AttributeGroupId",
                table: "AtrDefinition",
                column: "AttributeGroupId",
                principalTable: "AtrGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
