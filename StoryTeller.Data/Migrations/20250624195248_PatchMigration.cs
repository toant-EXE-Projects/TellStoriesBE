using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoryTeller.Data.Migrations
{
    /// <inheritdoc />
    public partial class PatchMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_AspNetUsers_CreatedById",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_AspNetUsers_DeleteById",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_AspNetUsers_UpdatedById",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLog_AspNetUsers_UserId",
                table: "ActivityLog");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcement_AspNetUsers_CreatedById",
                table: "Announcement");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcement_AspNetUsers_DeleteById",
                table: "Announcement");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcement_AspNetUsers_UpdatedById",
                table: "Announcement");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_CreatedById",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_DeleteById",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UpdatedById",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_ReplyTo",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Stories_StoryId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_CreatedById",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_DeleteById",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_UpdatedById",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_CreatedById",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_DeleteById",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_UpdatedById",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_UserId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Stories_StoryId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_AspNetUsers_CreatedById",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_AspNetUsers_DeleteById",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_AspNetUsers_UpdatedById",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_AspNetUsers_UserId",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_Stories_StoryId",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTag_Tag_TagId",
                table: "StoryTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_CreatedById",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_DeleteById",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_Tag_AspNetUsers_UpdatedById",
                table: "Tag");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLog_AspNetUsers_CreatedById",
                table: "UsageLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLog_AspNetUsers_DeleteById",
                table: "UsageLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLog_AspNetUsers_UpdatedById",
                table: "UsageLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLog_AspNetUsers_UserId",
                table: "UsageLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibrary_AspNetUsers_CreatedById",
                table: "UserLibrary");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibrary_AspNetUsers_DeleteById",
                table: "UserLibrary");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibrary_AspNetUsers_UpdatedById",
                table: "UserLibrary");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibrary_AspNetUsers_UserId",
                table: "UserLibrary");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_CreatedById",
                table: "UserLibraryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_DeleteById",
                table: "UserLibraryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_UpdatedById",
                table: "UserLibraryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItem_Stories_StoryId",
                table: "UserLibraryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItem_UserLibrary_UserCollectionId",
                table: "UserLibraryItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLibraryItem",
                table: "UserLibraryItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLibrary",
                table: "UserLibrary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsageLog",
                table: "UsageLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryTag",
                table: "StoryTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcement",
                table: "Announcement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog");

            migrationBuilder.RenameTable(
                name: "UserLibraryItem",
                newName: "UserLibraryItems");

            migrationBuilder.RenameTable(
                name: "UserLibrary",
                newName: "UserLibraries");

            migrationBuilder.RenameTable(
                name: "UsageLog",
                newName: "UsageLogs");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "StoryTag",
                newName: "StoryTags");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "Announcement",
                newName: "Announcements");

            migrationBuilder.RenameTable(
                name: "ActivityLog",
                newName: "ActivityLogs");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItem_UserCollectionId",
                table: "UserLibraryItems",
                newName: "IX_UserLibraryItems_UserCollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItem_UpdatedById",
                table: "UserLibraryItems",
                newName: "IX_UserLibraryItems_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItem_StoryId",
                table: "UserLibraryItems",
                newName: "IX_UserLibraryItems_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItem_DeleteById",
                table: "UserLibraryItems",
                newName: "IX_UserLibraryItems_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItem_CreatedById",
                table: "UserLibraryItems",
                newName: "IX_UserLibraryItems_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibrary_UserId",
                table: "UserLibraries",
                newName: "IX_UserLibraries_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibrary_UpdatedById",
                table: "UserLibraries",
                newName: "IX_UserLibraries_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibrary_DeleteById",
                table: "UserLibraries",
                newName: "IX_UserLibraries_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibrary_CreatedById",
                table: "UserLibraries",
                newName: "IX_UserLibraries_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLog_UserId",
                table: "UsageLogs",
                newName: "IX_UsageLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLog_UpdatedById",
                table: "UsageLogs",
                newName: "IX_UsageLogs_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLog_DeleteById",
                table: "UsageLogs",
                newName: "IX_UsageLogs_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLog_CreatedById",
                table: "UsageLogs",
                newName: "IX_UsageLogs_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_UpdatedById",
                table: "Tags",
                newName: "IX_Tags_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_DeleteById",
                table: "Tags",
                newName: "IX_Tags_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Tag_CreatedById",
                table: "Tags",
                newName: "IX_Tags_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_UserId",
                table: "StoryTags",
                newName: "IX_StoryTags_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_UpdatedById",
                table: "StoryTags",
                newName: "IX_StoryTags_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_StoryId",
                table: "StoryTags",
                newName: "IX_StoryTags_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_DeleteById",
                table: "StoryTags",
                newName: "IX_StoryTags_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTag_CreatedById",
                table: "StoryTags",
                newName: "IX_StoryTags_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_UserId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_UpdatedById",
                table: "Ratings",
                newName: "IX_Ratings_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_StoryId",
                table: "Ratings",
                newName: "IX_Ratings_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_DeleteById",
                table: "Ratings",
                newName: "IX_Ratings_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_CreatedById",
                table: "Ratings",
                newName: "IX_Ratings_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UpdatedById",
                table: "Notifications",
                newName: "IX_Notifications_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_DeleteById",
                table: "Notifications",
                newName: "IX_Notifications_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_CreatedById",
                table: "Notifications",
                newName: "IX_Notifications_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UpdatedById",
                table: "Comments",
                newName: "IX_Comments_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_StoryId",
                table: "Comments",
                newName: "IX_Comments_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ReplyTo",
                table: "Comments",
                newName: "IX_Comments_ReplyTo");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_DeleteById",
                table: "Comments",
                newName: "IX_Comments_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_CreatedById",
                table: "Comments",
                newName: "IX_Comments_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcement_UpdatedById",
                table: "Announcements",
                newName: "IX_Announcements_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcement_DeleteById",
                table: "Announcements",
                newName: "IX_Announcements_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcement_CreatedById",
                table: "Announcements",
                newName: "IX_Announcements_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_UserId",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_UpdatedById",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_DeleteById",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLog_CreatedById",
                table: "ActivityLogs",
                newName: "IX_ActivityLogs_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLibraryItems",
                table: "UserLibraryItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLibraries",
                table: "UserLibraries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsageLogs",
                table: "UsageLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryTags",
                table: "StoryTags",
                columns: new[] { "TagId", "StoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_CreatedById",
                table: "ActivityLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_DeleteById",
                table: "ActivityLogs",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UpdatedById",
                table: "ActivityLogs",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UserId",
                table: "ActivityLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_CreatedById",
                table: "Announcements",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_DeleteById",
                table: "Announcements",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_UpdatedById",
                table: "Announcements",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_DeleteById",
                table: "Comments",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UpdatedById",
                table: "Comments",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ReplyTo",
                table: "Comments",
                column: "ReplyTo",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_CreatedById",
                table: "Notifications",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_DeleteById",
                table: "Notifications",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UpdatedById",
                table: "Notifications",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_CreatedById",
                table: "Ratings",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_DeleteById",
                table: "Ratings",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UpdatedById",
                table: "Ratings",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Stories_StoryId",
                table: "Ratings",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_AspNetUsers_CreatedById",
                table: "StoryTags",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_AspNetUsers_DeleteById",
                table: "StoryTags",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_AspNetUsers_UpdatedById",
                table: "StoryTags",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_AspNetUsers_UserId",
                table: "StoryTags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_Stories_StoryId",
                table: "StoryTags",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTags_Tags_TagId",
                table: "StoryTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_CreatedById",
                table: "Tags",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_DeleteById",
                table: "Tags",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_UpdatedById",
                table: "Tags",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLogs_AspNetUsers_CreatedById",
                table: "UsageLogs",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLogs_AspNetUsers_DeleteById",
                table: "UsageLogs",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLogs_AspNetUsers_UpdatedById",
                table: "UsageLogs",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLogs_AspNetUsers_UserId",
                table: "UsageLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraries_AspNetUsers_CreatedById",
                table: "UserLibraries",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraries_AspNetUsers_DeleteById",
                table: "UserLibraries",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraries_AspNetUsers_UpdatedById",
                table: "UserLibraries",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraries_AspNetUsers_UserId",
                table: "UserLibraries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_CreatedById",
                table: "UserLibraryItems",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_DeleteById",
                table: "UserLibraryItems",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_UpdatedById",
                table: "UserLibraryItems",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItems_Stories_StoryId",
                table: "UserLibraryItems",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItems_UserLibraries_UserCollectionId",
                table: "UserLibraryItems",
                column: "UserCollectionId",
                principalTable: "UserLibraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_CreatedById",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_DeleteById",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UpdatedById",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityLogs_AspNetUsers_UserId",
                table: "ActivityLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_CreatedById",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_DeleteById",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_UpdatedById",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_DeleteById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UpdatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ReplyTo",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Stories_StoryId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_CreatedById",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_DeleteById",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UpdatedById",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_CreatedById",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_DeleteById",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UpdatedById",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Stories_StoryId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_AspNetUsers_CreatedById",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_AspNetUsers_DeleteById",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_AspNetUsers_UpdatedById",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_AspNetUsers_UserId",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_Stories_StoryId",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_StoryTags_Tags_TagId",
                table: "StoryTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_CreatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_DeleteById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_UpdatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLogs_AspNetUsers_CreatedById",
                table: "UsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLogs_AspNetUsers_DeleteById",
                table: "UsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLogs_AspNetUsers_UpdatedById",
                table: "UsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageLogs_AspNetUsers_UserId",
                table: "UsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraries_AspNetUsers_CreatedById",
                table: "UserLibraries");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraries_AspNetUsers_DeleteById",
                table: "UserLibraries");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraries_AspNetUsers_UpdatedById",
                table: "UserLibraries");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraries_AspNetUsers_UserId",
                table: "UserLibraries");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_CreatedById",
                table: "UserLibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_DeleteById",
                table: "UserLibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItems_AspNetUsers_UpdatedById",
                table: "UserLibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItems_Stories_StoryId",
                table: "UserLibraryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLibraryItems_UserLibraries_UserCollectionId",
                table: "UserLibraryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLibraryItems",
                table: "UserLibraryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLibraries",
                table: "UserLibraries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsageLogs",
                table: "UsageLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoryTags",
                table: "StoryTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLogs",
                table: "ActivityLogs");

            migrationBuilder.RenameTable(
                name: "UserLibraryItems",
                newName: "UserLibraryItem");

            migrationBuilder.RenameTable(
                name: "UserLibraries",
                newName: "UserLibrary");

            migrationBuilder.RenameTable(
                name: "UsageLogs",
                newName: "UsageLog");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "StoryTags",
                newName: "StoryTag");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameTable(
                name: "Announcements",
                newName: "Announcement");

            migrationBuilder.RenameTable(
                name: "ActivityLogs",
                newName: "ActivityLog");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItems_UserCollectionId",
                table: "UserLibraryItem",
                newName: "IX_UserLibraryItem_UserCollectionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItems_UpdatedById",
                table: "UserLibraryItem",
                newName: "IX_UserLibraryItem_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItems_StoryId",
                table: "UserLibraryItem",
                newName: "IX_UserLibraryItem_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItems_DeleteById",
                table: "UserLibraryItem",
                newName: "IX_UserLibraryItem_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraryItems_CreatedById",
                table: "UserLibraryItem",
                newName: "IX_UserLibraryItem_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraries_UserId",
                table: "UserLibrary",
                newName: "IX_UserLibrary_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraries_UpdatedById",
                table: "UserLibrary",
                newName: "IX_UserLibrary_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraries_DeleteById",
                table: "UserLibrary",
                newName: "IX_UserLibrary_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UserLibraries_CreatedById",
                table: "UserLibrary",
                newName: "IX_UserLibrary_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLogs_UserId",
                table: "UsageLog",
                newName: "IX_UsageLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLogs_UpdatedById",
                table: "UsageLog",
                newName: "IX_UsageLog_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLogs_DeleteById",
                table: "UsageLog",
                newName: "IX_UsageLog_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_UsageLogs_CreatedById",
                table: "UsageLog",
                newName: "IX_UsageLog_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_UpdatedById",
                table: "Tag",
                newName: "IX_Tag_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_DeleteById",
                table: "Tag",
                newName: "IX_Tag_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_CreatedById",
                table: "Tag",
                newName: "IX_Tag_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_UserId",
                table: "StoryTag",
                newName: "IX_StoryTag_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_UpdatedById",
                table: "StoryTag",
                newName: "IX_StoryTag_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_StoryId",
                table: "StoryTag",
                newName: "IX_StoryTag_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_DeleteById",
                table: "StoryTag",
                newName: "IX_StoryTag_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_StoryTags_CreatedById",
                table: "StoryTag",
                newName: "IX_StoryTag_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "Rating",
                newName: "IX_Rating_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UpdatedById",
                table: "Rating",
                newName: "IX_Rating_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_StoryId",
                table: "Rating",
                newName: "IX_Rating_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_DeleteById",
                table: "Rating",
                newName: "IX_Rating_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_CreatedById",
                table: "Rating",
                newName: "IX_Rating_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UpdatedById",
                table: "Notification",
                newName: "IX_Notification_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_DeleteById",
                table: "Notification",
                newName: "IX_Notification_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CreatedById",
                table: "Notification",
                newName: "IX_Notification_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comment",
                newName: "IX_Comment_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UpdatedById",
                table: "Comment",
                newName: "IX_Comment_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_StoryId",
                table: "Comment",
                newName: "IX_Comment_StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ReplyTo",
                table: "Comment",
                newName: "IX_Comment_ReplyTo");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_DeleteById",
                table: "Comment",
                newName: "IX_Comment_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CreatedById",
                table: "Comment",
                newName: "IX_Comment_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_UpdatedById",
                table: "Announcement",
                newName: "IX_Announcement_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_DeleteById",
                table: "Announcement",
                newName: "IX_Announcement_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_CreatedById",
                table: "Announcement",
                newName: "IX_Announcement_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_UserId",
                table: "ActivityLog",
                newName: "IX_ActivityLog_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_UpdatedById",
                table: "ActivityLog",
                newName: "IX_ActivityLog_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_DeleteById",
                table: "ActivityLog",
                newName: "IX_ActivityLog_DeleteById");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityLogs_CreatedById",
                table: "ActivityLog",
                newName: "IX_ActivityLog_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLibraryItem",
                table: "UserLibraryItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLibrary",
                table: "UserLibrary",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsageLog",
                table: "UsageLog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoryTag",
                table: "StoryTag",
                columns: new[] { "TagId", "StoryId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcement",
                table: "Announcement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_AspNetUsers_CreatedById",
                table: "ActivityLog",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_AspNetUsers_DeleteById",
                table: "ActivityLog",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_AspNetUsers_UpdatedById",
                table: "ActivityLog",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityLog_AspNetUsers_UserId",
                table: "ActivityLog",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcement_AspNetUsers_CreatedById",
                table: "Announcement",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcement_AspNetUsers_DeleteById",
                table: "Announcement",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcement_AspNetUsers_UpdatedById",
                table: "Announcement",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_CreatedById",
                table: "Comment",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_DeleteById",
                table: "Comment",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UpdatedById",
                table: "Comment",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_ReplyTo",
                table: "Comment",
                column: "ReplyTo",
                principalTable: "Comment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Stories_StoryId",
                table: "Comment",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_CreatedById",
                table: "Notification",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_DeleteById",
                table: "Notification",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_UpdatedById",
                table: "Notification",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_CreatedById",
                table: "Rating",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_DeleteById",
                table: "Rating",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_UpdatedById",
                table: "Rating",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_UserId",
                table: "Rating",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Stories_StoryId",
                table: "Rating",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_AspNetUsers_CreatedById",
                table: "StoryTag",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_AspNetUsers_DeleteById",
                table: "StoryTag",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_AspNetUsers_UpdatedById",
                table: "StoryTag",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_AspNetUsers_UserId",
                table: "StoryTag",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_Stories_StoryId",
                table: "StoryTag",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoryTag_Tag_TagId",
                table: "StoryTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_CreatedById",
                table: "Tag",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_DeleteById",
                table: "Tag",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tag_AspNetUsers_UpdatedById",
                table: "Tag",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLog_AspNetUsers_CreatedById",
                table: "UsageLog",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLog_AspNetUsers_DeleteById",
                table: "UsageLog",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLog_AspNetUsers_UpdatedById",
                table: "UsageLog",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageLog_AspNetUsers_UserId",
                table: "UsageLog",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibrary_AspNetUsers_CreatedById",
                table: "UserLibrary",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibrary_AspNetUsers_DeleteById",
                table: "UserLibrary",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibrary_AspNetUsers_UpdatedById",
                table: "UserLibrary",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibrary_AspNetUsers_UserId",
                table: "UserLibrary",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_CreatedById",
                table: "UserLibraryItem",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_DeleteById",
                table: "UserLibraryItem",
                column: "DeleteById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItem_AspNetUsers_UpdatedById",
                table: "UserLibraryItem",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItem_Stories_StoryId",
                table: "UserLibraryItem",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLibraryItem_UserLibrary_UserCollectionId",
                table: "UserLibraryItem",
                column: "UserCollectionId",
                principalTable: "UserLibrary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
