using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Yggdrasil.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("0a1b7f2c-0000-4000-8000-0000000000a1"), "0a1b7f2c-0000-4000-8000-0000000000b1", "Admin", "ADMIN" },
                    { new Guid("0a1b7f2c-0000-4000-8000-0000000000a2"), "0a1b7f2c-0000-4000-8000-0000000000b2", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0a1b7f2c-0000-4000-8000-0000000000a1"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("0a1b7f2c-0000-4000-8000-0000000000a2"));
        }
    }
}
