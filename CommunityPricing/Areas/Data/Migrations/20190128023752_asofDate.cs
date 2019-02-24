using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunityPricing.Data.Migrations
{
    public partial class asofDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AsOfDate",
                table: "Offering",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AsOfDate",
                table: "Offering");
        }
    }
}
