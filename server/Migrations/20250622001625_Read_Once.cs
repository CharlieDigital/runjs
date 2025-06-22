using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EFExample.Migrations
{
    /// <inheritdoc />
    public partial class Read_Once : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "read_once",
                table: "secrets",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "read_once",
                table: "secrets");
        }
    }
}
