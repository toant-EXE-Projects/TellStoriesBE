using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class LibraryAccessTypeSubscriptionOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscription_Subscription_SubscriptionId1",
                table: "UserSubscription");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscription_SubscriptionId1",
                table: "UserSubscription");

            migrationBuilder.DropColumn(
                name: "SubscriptionId1",
                table: "UserSubscription");

            migrationBuilder.AddColumn<int>(
                name: "AccessType",
                table: "UserLibraries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveSubscriptionId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ActiveSubscriptionId",
                table: "AspNetUsers",
                column: "ActiveSubscriptionId",
                unique: true,
                filter: "[ActiveSubscriptionId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserSubscription_ActiveSubscriptionId",
                table: "AspNetUsers",
                column: "ActiveSubscriptionId",
                principalTable: "UserSubscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserSubscription_ActiveSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ActiveSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AccessType",
                table: "UserLibraries");

            migrationBuilder.DropColumn(
                name: "ActiveSubscriptionId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId1",
                table: "UserSubscription",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscription_SubscriptionId1",
                table: "UserSubscription",
                column: "SubscriptionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscription_Subscription_SubscriptionId1",
                table: "UserSubscription",
                column: "SubscriptionId1",
                principalTable: "Subscription",
                principalColumn: "Id");
        }
    }
}
