namespace StoryTeller.Services.Models.RequestModel
{
    public class EmailSubscriptionReminderRequest
    {
        public string? Subject { get; set; }
        public string? SupportEmail { get; set; }
        public string Recipient { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string SubscriptionName { get; set; } = null!;
        public string SubscriptionExpiry { get; set; } = null!;

    }
}
