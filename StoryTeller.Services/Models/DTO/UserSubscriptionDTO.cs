using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class UserSubscriptionDTO
    {
        public string UserId { get; set; } = null!;
        public UserMinimalDTO User { get; set; } = null!;
        public Guid SubscriptionId { get; set; }
        public SubscriptionDTO Subscription { get; set; } = null!;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public DateTime? OriginalEndDate { get; set; }

        public bool? AutoRenew { get; set; }
    }
}
