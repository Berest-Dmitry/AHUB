using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainLayer.Migrations
{
    /// <inheritdoc />
    public partial class FilesAndPostsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileData",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        fileName = table.Column<string>(type: "text", nullable: false),
                        filePath = table.Column<string>(type: "text", nullable: false),
                        mediaType = table.Column<string>(type: "text", nullable: false),
                        fileSize = table.Column<string>(type: "text", nullable: true),
                        bucketName = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_FileData", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        title = table.Column<string>(type: "text", nullable: false),
                        content = table.Column<string>(type: "text", nullable: false),
                        publisherName = table.Column<string>(type: "text", nullable: true),
                        linkURL = table.Column<string>(type: "text", nullable: true),
                        linkName = table.Column<string>(type: "text", nullable: true),
                        geoTag = table.Column<string>(type: "text", nullable: true),
                        userId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PostFiles",
                columns: table =>
                    new
                    {
                        id = table.Column<Guid>(type: "uuid", nullable: false),
                        postId = table.Column<Guid>(type: "uuid", nullable: false),
                        fileId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_PostFiles", x => x.id);
                    table.ForeignKey(
                        name: "FK_PostFiles_Posts_postId",
                        column: x => x.postId,
                        principalTable: "Posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PostFiles_postId",
                table: "PostFiles",
                column: "postId"
            );

            migrationBuilder.CreateIndex(name: "IX_Posts_userId", table: "Posts", column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FileData");

            migrationBuilder.DropTable(name: "PostFiles");

            migrationBuilder.DropTable(name: "Posts");
        }
    }
}
