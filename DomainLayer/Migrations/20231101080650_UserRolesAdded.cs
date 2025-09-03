using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class UserRolesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        userId = table.Column<Guid>(type: "uuid", nullable: false),
                        roleId = table.Column<Guid>(type: "uuid", nullable: false),
                        dateTimeAdded = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        deletedAt = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_roleId",
                        column: x => x.roleId,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_roleId",
                table: "UserRoles",
                column: "roleId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_userId",
                table: "UserRoles",
                column: "userId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserRoles");
        }
    }
}
