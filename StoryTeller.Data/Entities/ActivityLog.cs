using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class ActivityLog : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string TargetType { get; set; } = null!;
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public string? Details { get; set; }
        public string? Category { get; set; }
        public Guid? TargetId { get; set; }
        public string? Reason { get; set; }
        public string? ActorRole { get; set; }
        public string? DeviceInfo { get; set; }
    }
}
