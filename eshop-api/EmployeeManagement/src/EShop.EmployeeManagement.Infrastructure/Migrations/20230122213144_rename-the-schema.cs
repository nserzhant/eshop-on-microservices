using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EShop.EmployeeManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class renametheschema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "users",
                newName: "AspNetUserTokens",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "users",
                newName: "AspNetUsers",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "users",
                newName: "AspNetUserRoles",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "users",
                newName: "AspNetUserLogins",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "users",
                newName: "AspNetUserClaims",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "users",
                newName: "AspNetRoles",
                newSchema: "employee");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "users",
                newName: "AspNetRoleClaims",
                newSchema: "employee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "employee",
                newName: "AspNetUserTokens",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "employee",
                newName: "AspNetUsers",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "employee",
                newName: "AspNetUserRoles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "employee",
                newName: "AspNetUserLogins",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "employee",
                newName: "AspNetUserClaims",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "employee",
                newName: "AspNetRoles",
                newSchema: "users");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "employee",
                newName: "AspNetRoleClaims",
                newSchema: "users");
        }
    }
}
