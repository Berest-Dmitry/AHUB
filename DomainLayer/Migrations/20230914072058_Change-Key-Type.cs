using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKeyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "recoveryKey",
                table: "PasswordChanges",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "recoveryKey",
                table: "PasswordChanges",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer"
            );
        }
    }
}
