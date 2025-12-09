namespace StoryTeller.Data.Entities
{
    public class NotificationRead : BaseEntity
    {
        public Guid NotificationId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime ReadAt { get; set; }

        public Notification Notification { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
