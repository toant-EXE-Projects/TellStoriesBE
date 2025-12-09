namespace StoryTeller.Services.Models.RequestModel
{
    public class NotificationSendRequest
    {
        public string? UserId { get; set; }

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!; // e.g., "System", "Reminder"
        public string? Sender { get; set; }

        public string? TargetType { get; set; } // e.g., "Story", "Comment"
    }
}
