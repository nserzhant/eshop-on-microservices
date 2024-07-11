using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.EmployeeManagement.Infrastructure.Migrations
{
    public partial class create_predefined_roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"INSERT INTO [users].[AspNetRoles]([Id],[Name],[NormalizedName])
                    VALUES(CAST(NewId() as NVARCHAR(450)), 'Administrator', 'ADMINISTRATOR'),
                           (CAST(NewId() as NVARCHAR(450)), 'SalesManager', 'SALESMANAGER'); ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [users].[AspNetRoles];");
        }
    }
}
