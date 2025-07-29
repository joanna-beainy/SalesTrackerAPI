using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesTracker.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageLimitsToRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxUsageCount",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "RefreshTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUsageCount",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "RefreshTokens");
        }
    }
}
