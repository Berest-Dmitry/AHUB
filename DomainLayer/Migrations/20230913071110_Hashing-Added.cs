using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class HashingAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "hashedPassword"
            );

            migrationBuilder.AddColumn<string>(
                name: "salt",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "salt", table: "Users");

            migrationBuilder.RenameColumn(
                name: "hashedPassword",
                table: "Users",
                newName: "password"
            );
        }
    }
}
