using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_idempotent_commands_infrastructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumedIntegrationCommands",
                schema: "catalog",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumedIntegrationCommands", x => x.MessageId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumedIntegrationCommands",
                schema: "catalog");
        }
    }
}
