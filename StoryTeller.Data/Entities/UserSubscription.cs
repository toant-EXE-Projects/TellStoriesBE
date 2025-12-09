using StoryTeller.Data.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class UserSubscription : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public DateTime? OriginalEndDate { get; set; }

        public string? Status { get; set; }
        public bool? HasNotified { get; set; }
        public bool? AutoRenew { get; set; }
        public bool IsActive(DateTime now)
        {
            return EndDate.HasValue &&
                   EndDate.Value >= now &&
                   (Status == null || Status == SubscriptionConstants.StatusActive);
        }
    }
}
