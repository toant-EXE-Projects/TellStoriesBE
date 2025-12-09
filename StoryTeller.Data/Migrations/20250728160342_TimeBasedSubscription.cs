using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class TimeBasedSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalDate",
                table: "UserSubscription");

            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "MaxAIRequest",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "MaxStories",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "MaxTTSRequest",
                table: "Subscription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RenewalDate",
                table: "UserSubscription",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingCycle",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAIRequest",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxStories",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTTSRequest",
                table: "Subscription",
                type: "int",
                nullable: true);
        }
    }
}
