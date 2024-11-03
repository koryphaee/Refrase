using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refrase.Model.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "videos",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    category = table.Column<string>(type: "TEXT", nullable: false),
                    imported = table.Column<DateTime>(type: "TEXT", nullable: false),
                    url = table.Column<string>(type: "TEXT", nullable: true),
                    status = table.Column<int>(type: "INTEGER", nullable: false),
                    width = table.Column<int>(type: "INTEGER", nullable: false),
                    height = table.Column<int>(type: "INTEGER", nullable: false),
                    frame_count = table.Column<int>(type: "INTEGER", nullable: false),
                    frame_rate = table.Column<float>(type: "REAL", nullable: false),
                    duration = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_videos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "frames",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    video_id = table.Column<long>(type: "INTEGER", nullable: false),
                    index = table.Column<int>(type: "INTEGER", nullable: false),
                    timestamp = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    hash = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_frames", x => x.id);
                    table.ForeignKey(
                        name: "fk_frames_videos_video_id",
                        column: x => x.video_id,
                        principalTable: "videos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_frames_video_id_index",
                table: "frames",
                columns: new[] { "video_id", "index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_videos_name",
                table: "videos",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "frames");

            migrationBuilder.DropTable(
                name: "videos");
        }
    }
}
