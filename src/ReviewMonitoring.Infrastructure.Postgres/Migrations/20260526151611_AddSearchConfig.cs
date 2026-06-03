using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewMonitoring.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchConfig",
                table: "Jobs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchConfig",
                table: "Jobs");
        }
    }
}
