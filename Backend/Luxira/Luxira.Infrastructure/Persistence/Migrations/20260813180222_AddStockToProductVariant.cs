using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Luxira.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "ProductVariants");
        }
    }
}
