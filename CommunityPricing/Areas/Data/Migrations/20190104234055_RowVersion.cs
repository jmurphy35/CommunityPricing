using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunityPricing.Data.Migrations
{
    public partial class RowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Vendor",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ProductCategory",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Product",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Offering",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Vendor");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProductCategory");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Offering");
        }
    }
}
