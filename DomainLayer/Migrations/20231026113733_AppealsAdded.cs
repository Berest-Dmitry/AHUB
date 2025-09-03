using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class AppealsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appeals",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        reason = table.Column<short>(type: "smallint", nullable: false),
                        status = table.Column<short>(type: "smallint", nullable: false),
                        comment = table.Column<string>(type: "text", nullable: true),
                        userId = table.Column<Guid>(type: "uuid", nullable: false),
                        appealEntityId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Appeals", x => x.id);
                    table.ForeignKey(
                        name: "FK_Appeals_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Appeals_userId",
                table: "Appeals",
                column: "userId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Appeals");
        }
    }
}
