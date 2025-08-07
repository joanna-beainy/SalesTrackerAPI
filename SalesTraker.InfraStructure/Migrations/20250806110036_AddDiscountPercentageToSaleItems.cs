using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesTracker.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountPercentageToSaleItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountPercentage",
                table: "SaleItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "SaleItems");
        }
    }
}
