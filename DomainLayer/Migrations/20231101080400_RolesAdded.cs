using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class RolesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        name = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_Roles", x => x.id);
                }
            );

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "id", "name", "dateTimeAdded", "deletedAt" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Administrator", DateTime.UtcNow, null },
                    { Guid.NewGuid(), "User", DateTime.UtcNow, null }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Roles");
        }
    }
}
