using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class PasswordChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordChanges",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        userId = table.Column<Guid>(type: "uuid", nullable: false),
                        recoveryKey = table.Column<short>(type: "smallint", nullable: false),
                        keyValidBefore = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        changePasswordBefore = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
                        dateTimeAdded = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordChanges", x => x.id);
                    table.ForeignKey(
                        name: "FK_PasswordChanges_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PasswordChanges_userId",
                table: "PasswordChanges",
                column: "userId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PasswordChanges");
        }
    }
}
