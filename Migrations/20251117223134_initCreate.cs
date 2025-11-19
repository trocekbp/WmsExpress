using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Store_Warehouse_App.Migrations
{
    /// <inheritdoc />
    public partial class initCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "FeatureDefinition",
                columns: table => new
                {
                    FeatureDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureDefinition", x => x.FeatureDefinitionId);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                columns: table => new
                {
                    InstrumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EAN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.InstrumentId);
                    table.ForeignKey(
                        name: "FK_Instrument_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Instrument_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DocumentInstrument",
                columns: table => new
                {
                    DocumentInstrumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentInstrument", x => x.DocumentInstrumentId);
                    table.ForeignKey(
                        name: "FK_DocumentInstrument_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentInstrument_Instrument_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentFeature",
                columns: table => new
                {
                    InstrumentFeatureId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    FeatureDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentFeature", x => x.InstrumentFeatureId);
                    table.ForeignKey(
                        name: "FK_InstrumentFeature_FeatureDefinition_FeatureDefinitionId",
                        column: x => x.FeatureDefinitionId,
                        principalTable: "FeatureDefinition",
                        principalColumn: "FeatureDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstrumentFeature_Instrument_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentInventory",
                columns: table => new
                {
                    InstrumentInventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstrumentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentInventory", x => x.InstrumentInventoryId);
                    table.ForeignKey(
                        name: "FK_InstrumentInventory_Instrument_InstrumentId",
                        column: x => x.InstrumentId,
                        principalTable: "Instrument",
                        principalColumn: "InstrumentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_SupplierId",
                table: "Address",
                column: "SupplierId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInstrument_DocumentId",
                table: "DocumentInstrument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentInstrument_InstrumentId",
                table: "DocumentInstrument",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_CategoryId",
                table: "Instrument",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_SupplierId",
                table: "Instrument",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentFeature_FeatureDefinitionId",
                table: "InstrumentFeature",
                column: "FeatureDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentFeature_InstrumentId",
                table: "InstrumentFeature",
                column: "InstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentInventory_InstrumentId",
                table: "InstrumentInventory",
                column: "InstrumentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "DocumentInstrument");

            migrationBuilder.DropTable(
                name: "InstrumentFeature");

            migrationBuilder.DropTable(
                name: "InstrumentInventory");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "FeatureDefinition");

            migrationBuilder.DropTable(
                name: "Instrument");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
