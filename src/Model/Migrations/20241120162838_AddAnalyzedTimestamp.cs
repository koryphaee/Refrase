using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refrase.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyzedTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "analyzed",
                table: "videos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql("UPDATE videos SET analyzed = DATETIME(imported, '+1 minute') WHERE status = 4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "analyzed",
                table: "videos");
        }
    }
}
