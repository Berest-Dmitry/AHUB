using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        userName = table.Column<string>(
                            type: "character varying(50)",
                            maxLength: 50,
                            nullable: false
                        ),
                        password = table.Column<string>(
                            type: "character varying(50)",
                            maxLength: 50,
                            nullable: false
                        ),
                        firstName = table.Column<string>(
                            type: "character varying(50)",
                            maxLength: 50,
                            nullable: false
                        ),
                        lastName = table.Column<string>(
                            type: "character varying(50)",
                            maxLength: 50,
                            nullable: false
                        ),
                        birthday = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        ),
                        gender = table.Column<short>(type: "smallint", nullable: false),
                        educationInfo = table.Column<string>(type: "text", nullable: false),
                        jobInfo = table.Column<string>(type: "text", nullable: false),
                        homeTown = table.Column<string>(type: "text", nullable: false),
                        dateTimeAdded = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
