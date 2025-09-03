using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class HashtagsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HashTags",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        content = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_HashTags", x => x.id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "HashTags");
        }
    }
}
