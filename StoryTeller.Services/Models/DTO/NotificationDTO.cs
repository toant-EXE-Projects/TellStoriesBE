using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public UserMinimalDTO? User { get; set; }

        public bool IsRead { get; set; }

        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!; // e.g., "System", "Reminder"
        public string? Sender { get; set; }

        public string? TargetType { get; set; } // e.g., "Story", "Comment"
        public DateTime SentAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
