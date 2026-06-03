using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewMonitoring.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceStatusesToJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceStatuses",
                table: "Jobs",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceStatuses",
                table: "Jobs");
        }
    }
}
