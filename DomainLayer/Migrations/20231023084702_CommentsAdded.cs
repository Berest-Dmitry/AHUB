using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class CommentsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        content = table.Column<string>(
                            type: "character varying(500)",
                            maxLength: 500,
                            nullable: false
                        ),
                        userId = table.Column<Guid>(type: "uuid", nullable: false),
                        postId = table.Column<Guid>(type: "uuid", nullable: false),
                        parentId = table.Column<Guid>(type: "uuid", nullable: true),
                        dateTimeEdited = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: true
                        ),
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
                    table.PrimaryKey("PK_Comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_postId",
                        column: x => x.postId,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_Comments_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comments_postId",
                table: "Comments",
                column: "postId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Comments_userId",
                table: "Comments",
                column: "userId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Comments");
        }
    }
}
