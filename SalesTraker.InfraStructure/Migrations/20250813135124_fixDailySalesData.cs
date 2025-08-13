using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesTracker.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class fixDailySalesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailySalesReport",
                columns: table => new
                {
                    TotalSales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantitySold = table.Column<int>(type: "int", nullable: false),
                    TopProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TopProductQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySalesReport");
        }
    }
}
