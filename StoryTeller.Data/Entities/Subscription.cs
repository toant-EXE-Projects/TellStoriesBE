using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class Subscription : BaseEntity
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; } = 0;
        public string Type { get; set; } = null!;

        public int DurationDays { get; set; } 
        //public int? BillingCycle { get; set; } 

        //public int? MaxStories { get; set; } 
        //public int? MaxAIRequest { get; set; } 
        //public int? MaxTTSRequest { get; set; } 

        public bool? IsActive { get; set; } 

        public int? PointsCost { get; set; }
        public int RewardPoints { get; set; } = 0;

        public SubscriptionPurchaseMethod PurchaseMethod { get; set; } = SubscriptionPurchaseMethod.MoneyOnly;

        public ICollection<UserSubscription> UserSubscriptions { get; set; }


        public bool IsCurrentlyActive => IsActive.HasValue && IsActive.Value;
    }
}
