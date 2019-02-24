using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunityPricing.Data.Migrations
{
    public partial class CPContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    ProductCategoryID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.ProductCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    VendorID = table.Column<int>(nullable: false),
                    OwnerID = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    VendorAddress1 = table.Column<string>(nullable: true),
                    VendorAddress2 = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.VendorID);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductID = table.Column<Guid>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 50, nullable: true),
                    DescrOne = table.Column<string>(maxLength: 50, nullable: true),
                    Weight_Volume = table.Column<string>(maxLength: 20, nullable: true),
                    Wholesaler = table.Column<string>(maxLength: 50, nullable: true),
                    ProductCategoryID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_ProductCategoryID",
                        column: x => x.ProductCategoryID,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offering",
                columns: table => new
                {
                    OfferingID = table.Column<Guid>(nullable: false),
                    ProductID = table.Column<Guid>(nullable: false),
                    VendorID = table.Column<int>(nullable: false),
                    ProductPricePerWeight = table.Column<decimal>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offering", x => x.OfferingID);
                    table.ForeignKey(
                        name: "FK_Offering_Product_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Offering_Vendor_VendorID",
                        column: x => x.VendorID,
                        principalTable: "Vendor",
                        principalColumn: "VendorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offering_ProductID",
                table: "Offering",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Offering_VendorID",
                table: "Offering",
                column: "VendorID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategoryID",
                table: "Product",
                column: "ProductCategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offering");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "ProductCategory");
        }
    }
}
