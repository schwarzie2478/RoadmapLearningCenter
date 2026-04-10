using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learning.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaygroundSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Output = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Errors = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaygroundSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoadmapTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Track = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Prerequisites = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadmapTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadmapTopics_RoadmapTopics_ParentId",
                        column: x => x.ParentId,
                        principalTable: "RoadmapTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LearnerTopicStates",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: false),
                    LastVisitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NeedsReview = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearnerTopicStates", x => new { x.UserId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_LearnerTopicStates_RoadmapTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "RoadmapTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SeedPrompt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsGeneratedOnDemand = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicBlocks_RoadmapTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "RoadmapTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParagraphThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentParagraphId = table.Column<Guid>(type: "uuid", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    Depth = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParagraphThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParagraphThreads_ParagraphThreads_ParentParagraphId",
                        column: x => x.ParentParagraphId,
                        principalTable: "ParagraphThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParagraphThreads_TopicBlocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "TopicBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearnerTopicStates_TopicId",
                table: "LearnerTopicStates",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ParagraphThreads_BlockId",
                table: "ParagraphThreads",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_ParagraphThreads_ParentParagraphId",
                table: "ParagraphThreads",
                column: "ParentParagraphId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaygroundSessions_UserId",
                table: "PlaygroundSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaygroundSessions_UserId_TopicId",
                table: "PlaygroundSessions",
                columns: new[] { "UserId", "TopicId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoadmapTopics_ParentId",
                table: "RoadmapTopics",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicBlocks_TopicId",
                table: "TopicBlocks",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearnerTopicStates");

            migrationBuilder.DropTable(
                name: "ParagraphThreads");

            migrationBuilder.DropTable(
                name: "PlaygroundSessions");

            migrationBuilder.DropTable(
                name: "TopicBlocks");

            migrationBuilder.DropTable(
                name: "RoadmapTopics");
        }
    }
}
