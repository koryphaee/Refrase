using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Refrase.Model.Migrations
{
    /// <inheritdoc />
    public partial class ChangedStatusValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql("UPDATE videos SET status = status + 1 WHERE status > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
