using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class issueReportType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReport_AspNetUsers_CreatedById",
                table: "IssueReport");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReport_AspNetUsers_DeleteById",
                table: "IssueReport");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReport_AspNetUsers_UpdatedById",
                table: "IssueReport");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReport_AspNetUsers_UserId",
                table: "IssueReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueReport",
                table: "IssueReport");

            migrationBuilder.RenameTable(
                name: "IssueReport",
                newName: "IssueReports");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReport_UserId",
                table: "IssueReports",
                newName: "IX_IssueReports_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReport_UpdatedById",
                table: "IssueReports",
                newName: "IX_IssueReports_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReport_DeleteById",
                table: "IssueReports",
                newName: "IX_IssueReports_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReport_CreatedById",
                table: "IssueReports",
                newName: "IX_IssueReports_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "TargetId",
                table: "IssueReports",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "IssueReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueReports",
                table: "IssueReports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_CreatedById",
                table: "IssueReports",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_DeleteById",
                table: "IssueReports",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_UpdatedById",
                table: "IssueReports",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_AspNetUsers_UserId",
                table: "IssueReports",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_CreatedById",
                table: "IssueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_DeleteById",
                table: "IssueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_UpdatedById",
                table: "IssueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_AspNetUsers_UserId",
                table: "IssueReports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IssueReports",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "IssueReports");

            migrationBuilder.RenameTable(
                name: "IssueReports",
                newName: "IssueReport");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_UserId",
                table: "IssueReport",
                newName: "IX_IssueReport_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_UpdatedById",
                table: "IssueReport",
                newName: "IX_IssueReport_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_DeleteById",
                table: "IssueReport",
                newName: "IX_IssueReport_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_CreatedById",
                table: "IssueReport",
                newName: "IX_IssueReport_CreatedById");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetId",
                table: "IssueReport",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IssueReport",
                table: "IssueReport",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReport_AspNetUsers_CreatedById",
                table: "IssueReport",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReport_AspNetUsers_DeleteById",
                table: "IssueReport",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReport_AspNetUsers_UpdatedById",
                table: "IssueReport",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReport_AspNetUsers_UserId",
                table: "IssueReport",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
