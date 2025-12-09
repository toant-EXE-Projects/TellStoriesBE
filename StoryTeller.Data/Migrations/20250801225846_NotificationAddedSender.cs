using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotificationAddedSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sender",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sender",
                table: "Notifications");
        }
    }
}
