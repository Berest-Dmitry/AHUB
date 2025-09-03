using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class PasswordChangeTimeAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "passwordChangeTime",
                table: "PasswordChanges",
                type: "timestamp with time zone",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "passwordChangeTime", table: "PasswordChanges");
        }
    }
}
