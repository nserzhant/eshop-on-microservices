using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace R2S.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addavailableqtytocatalogitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableQty",
                schema: "catalog",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQty",
                schema: "catalog",
                table: "CatalogItems");
        }
    }
}
