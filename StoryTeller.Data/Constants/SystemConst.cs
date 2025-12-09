namespace StoryTeller.Data.Constants
{
    public static class SystemConst
    {
        // See Enums/eStoryType
        public const int StoryType = 2;

        public const int Comment_MaxDepth = 3;

        public const int Min_Admins = 2;

        public static class Keys
        {
            public const string Login_DailyReward_Points = "Login_DailyReward_Points";

            public const string StoryPublish_MaxPendingRequests_Default = "StoryPublish_MaxPendingRequests_Default";
            public const string StoryPublish_MaxPendingRequests_Tier1 = "StoryPublish_MaxPendingRequests_Tier1";

            public const string Subscription_Background_CheckExpiration_IntervalMinutes = "Subscription_Background_CheckExpiration_IntervalMinutes";
            public const string Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes = "Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes";
            public const string Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration = "Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration";

            public const string BillingRecord_Background_CheckRecords_IntervalMinutes = "BillingRecord_Background_CheckRecords_IntervalMinutes";
            public const string BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes = "BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes";
        }

        public static class Values
        {
            public const int Login_DailyReward_Points = 5;

            public const int StoryPublish_MaxPendingRequests_Default = 5;
            public const int StoryPublish_MaxPendingRequests_Tier1 = 15;

            public const int Subscription_Background_CheckExpiration_IntervalMinutes = 15;
            public const int Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes = 15;
            public const int Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration = 3;

            public const int BillingRecord_Background_CheckRecords_IntervalMinutes = 15;
            public const int BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes = 30;
        }
    }
}
