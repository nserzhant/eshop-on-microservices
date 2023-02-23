using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace R2S.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createuniqueconstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CatalogTypes_Type",
                schema: "catalog",
                table: "CatalogTypes",
                column: "Type",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_Name_CatalogBrandId_CatalogTypeId",
                schema: "catalog",
                table: "CatalogItems",
                columns: new[] { "Name", "CatalogBrandId", "CatalogTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBrands_Brand",
                schema: "catalog",
                table: "CatalogBrands",
                column: "Brand",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogTypes_Type",
                schema: "catalog",
                table: "CatalogTypes");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_Name_CatalogBrandId_CatalogTypeId",
                schema: "catalog",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogBrands_Brand",
                schema: "catalog",
                table: "CatalogBrands");
        }
    }
}
