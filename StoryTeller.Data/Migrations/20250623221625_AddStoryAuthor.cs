using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStoryAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Stories");
        }
    }
}
