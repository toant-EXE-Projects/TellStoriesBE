using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyStoryPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioResourceId",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "ImageResourceId",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "TextResourceId",
                table: "StoryPanels");

            migrationBuilder.AddColumn<string>(
                name: "AudioUrl",
                table: "StoryPanels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "StoryPanels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "StoryPanels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUrl",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "StoryPanels");

            migrationBuilder.AddColumn<Guid>(
                name: "AudioResourceId",
                table: "StoryPanels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageResourceId",
                table: "StoryPanels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TextResourceId",
                table: "StoryPanels",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
