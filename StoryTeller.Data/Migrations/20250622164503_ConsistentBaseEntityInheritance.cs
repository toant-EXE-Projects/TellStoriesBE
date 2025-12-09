using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConsistentBaseEntityInheritance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "StoryPanels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "StoryPanels",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeleteById",
                table: "StoryPanels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionDate",
                table: "StoryPanels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoryPanels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StoryPanels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "StoryPanels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoryPanels_CreatedById",
                table: "StoryPanels",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPanels_DeleteById",
                table: "StoryPanels",
                column: "DeleteById");

            migrationBuilder.CreateIndex(
                name: "IX_StoryPanels_UpdatedById",
                table: "StoryPanels",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_StoryPanels_AspNetUsers_CreatedById",
                table: "StoryPanels",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryPanels_AspNetUsers_DeleteById",
                table: "StoryPanels",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryPanels_AspNetUsers_UpdatedById",
                table: "StoryPanels",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoryPanels_AspNetUsers_CreatedById",
                table: "StoryPanels");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryPanels_AspNetUsers_DeleteById",
                table: "StoryPanels");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryPanels_AspNetUsers_UpdatedById",
                table: "StoryPanels");

            migrationBuilder.DropIndex(
                name: "IX_StoryPanels_CreatedById",
                table: "StoryPanels");

            migrationBuilder.DropIndex(
                name: "IX_StoryPanels_DeleteById",
                table: "StoryPanels");

            migrationBuilder.DropIndex(
                name: "IX_StoryPanels_UpdatedById",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "DeleteById",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "DeletionDate",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StoryPanels");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "StoryPanels");
        }
    }
}
