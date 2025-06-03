using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_ticaret.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordAndDbPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$k7R6h.Ts3MFvHJpaq0HuUuCUYz6TtxMQJqhN9V3UF9T3HJGQZsuHe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$ej7Qj1JaT0dMFmKyy0xwkOYYvq7PJHtAeGZJXgJQwYm4Xa9jHnqji");
        }
    }
}
