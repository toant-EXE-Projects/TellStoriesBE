using System.Text.Json.Serialization;

namespace StoryTeller.Services.Background
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BackgroundTaskType
    {
        BillingRecords,
        SubscriptionEmailReminder,
        SubscriptionExpiration
    }
    public interface IBackgroundTaskToggle
    {
        bool BillingRecordsEnabled { get; set; }
        bool SubscriptionEmailReminderEnabled { get; set; }
        bool SubscriptionExpirationBGServiceEnabled { get; set; }

        bool IsEnabled(BackgroundTaskType taskType);
        void SetEnabled(BackgroundTaskType taskType, bool enabled);
        IReadOnlyDictionary<BackgroundTaskType, bool> GetAllTasks();
    }

    public class BackgroundTaskToggle : IBackgroundTaskToggle
    {
        public bool BillingRecordsEnabled { get; set; } = true;
        public bool SubscriptionEmailReminderEnabled { get; set; } = true;
        public bool SubscriptionExpirationBGServiceEnabled { get; set; } = true;

        public bool IsEnabled(BackgroundTaskType taskType) => taskType switch
        {
            BackgroundTaskType.BillingRecords => BillingRecordsEnabled,
            BackgroundTaskType.SubscriptionEmailReminder => SubscriptionEmailReminderEnabled,
            BackgroundTaskType.SubscriptionExpiration => SubscriptionExpirationBGServiceEnabled,
            _ => false
        };

        public void SetEnabled(BackgroundTaskType taskType, bool enabled)
        {
            switch (taskType)
            {
                case BackgroundTaskType.BillingRecords:
                    BillingRecordsEnabled = enabled;
                    break;
                case BackgroundTaskType.SubscriptionEmailReminder:
                    SubscriptionEmailReminderEnabled = enabled;
                    break;
                case BackgroundTaskType.SubscriptionExpiration:
                    SubscriptionExpirationBGServiceEnabled = enabled;
                    break;
            }
        }

        public IReadOnlyDictionary<BackgroundTaskType, bool> GetAllTasks()
        {
            return new Dictionary<BackgroundTaskType, bool>
            {
                { BackgroundTaskType.BillingRecords, BillingRecordsEnabled },
                { BackgroundTaskType.SubscriptionEmailReminder, SubscriptionEmailReminderEnabled },
                { BackgroundTaskType.SubscriptionExpiration, SubscriptionExpirationBGServiceEnabled }
            };
        }

    }
}
