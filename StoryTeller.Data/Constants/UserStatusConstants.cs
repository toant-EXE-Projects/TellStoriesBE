namespace StoryTeller.Data.Constants
{
    public static class UserStatusConstants
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
        public const string Disabled = "Disabled";
        public const string Suspended = "Suspended";
        public const string Banned = "Banned";
        public const string Deleted = "Deleted";

        public const string DeletedName = "[Deleted]";
        public const string DeletedUserName = @"deleted_{0}";
        public const string DeletedEmail = @"deleted_{0}@tellstories.invalid";
    }
}
