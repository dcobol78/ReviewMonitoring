using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewMonitoring.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsCollectedAndListingCollectedToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingsCollected",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCollected",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingsCollected",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ReviewsCollected",
                table: "Jobs");
        }
    }
}
