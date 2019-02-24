using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CommunityPricing.Data.Migrations
{
    public partial class ArchivedOffering_andMisc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPricePerWeight",
                table: "Offering",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.CreateTable(
                name: "ArchivedOffering",
                columns: table => new
                {
                    ArchivedOfferingID = table.Column<Guid>(nullable: false),
                    OfferingID = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Price = table.Column<decimal>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedOffering", x => x.ArchivedOfferingID);
                    table.ForeignKey(
                        name: "FK_ArchivedOffering_Offering_OfferingID",
                        column: x => x.OfferingID,
                        principalTable: "Offering",
                        principalColumn: "OfferingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedOffering_OfferingID",
                table: "ArchivedOffering",
                column: "OfferingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedOffering");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProductPricePerWeight",
                table: "Offering",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
