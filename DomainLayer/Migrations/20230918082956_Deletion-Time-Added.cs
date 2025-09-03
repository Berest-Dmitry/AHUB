using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class DeletionTimeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "deletedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deletedAt",
                table: "PasswordChanges",
                type: "timestamp with time zone",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "deletedAt", table: "Users");

            migrationBuilder.DropColumn(name: "deletedAt", table: "PasswordChanges");
        }
    }
}
