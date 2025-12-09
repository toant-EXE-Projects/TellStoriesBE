using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class BillingRecordTweaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingRecords_Subscription_SubscriptionId",
                table: "BillingRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "BillingRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "BillingRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillingRecords_Subscription_SubscriptionId",
                table: "BillingRecords",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingRecords_Subscription_SubscriptionId",
                table: "BillingRecords");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "BillingRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubscriptionId",
                table: "BillingRecords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingRecords_Subscription_SubscriptionId",
                table: "BillingRecords",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");
        }
    }
}
