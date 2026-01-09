using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Store_Warehouse_App.Migrations
{
    /// <inheritdoc />
    public partial class ItemsModernizationToArticlesWithGrossPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attribute_Item_ItemId",
                table: "Attribute");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItem_Item_ItemId",
                table: "DocumentItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_Item_ItemId",
                table: "InventoryMovement");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "InventoryMovement",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryMovement_ItemId",
                table: "InventoryMovement",
                newName: "IX_InventoryMovement_ArticleId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "DocumentItem",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentItem_ItemId",
                table: "DocumentItem",
                newName: "IX_DocumentItem_ArticleId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Attribute",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Attribute_ItemId",
                table: "Attribute",
                newName: "IX_Attribute_ArticleId");

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_Article_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_CategoryId",
                table: "Article",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_Code",
                table: "Article",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attribute_Article_ArticleId",
                table: "Attribute",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItem_Article_ArticleId",
                table: "DocumentItem",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_Article_ArticleId",
                table: "InventoryMovement",
                column: "ArticleId",
                principalTable: "Article",
                principalColumn: "ArticleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attribute_Article_ArticleId",
                table: "Attribute");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentItem_Article_ArticleId",
                table: "DocumentItem");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovement_Article_ArticleId",
                table: "InventoryMovement");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "InventoryMovement",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryMovement_ArticleId",
                table: "InventoryMovement",
                newName: "IX_InventoryMovement_ItemId");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "DocumentItem",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentItem_ArticleId",
                table: "DocumentItem",
                newName: "IX_DocumentItem_ItemId");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Attribute",
                newName: "ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Attribute_ArticleId",
                table: "Attribute",
                newName: "IX_Attribute_ItemId");

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Item_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Acronym",
                table: "Item",
                column: "Acronym",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryId",
                table: "Item",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attribute_Item_ItemId",
                table: "Attribute",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentItem_Item_ItemId",
                table: "DocumentItem",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovement_Item_ItemId",
                table: "InventoryMovement",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
