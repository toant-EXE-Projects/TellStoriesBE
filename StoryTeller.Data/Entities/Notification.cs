using System.ComponentModel.DataAnnotations.Schema;

namespace StoryTeller.Data.Entities
{
    public class Notification : BaseEntity
    {
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!; // e.g., "System", "Reminder"
        public string Sender { get; set; } = null!;

        public string? TargetType { get; set; } // e.g., "Story", "Comment"
        public DateTime SentAt { get; set; }

        [NotMapped]
        public bool IsRead { get; set; }

        public ICollection<NotificationRead> Reads { get; set; } = new List<NotificationRead>();
    }
}
