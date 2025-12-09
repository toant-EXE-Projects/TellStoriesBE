using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class SubscriptionDashboardResponse
    {
        public int SubscriptionRevenue { get; set; } = 0;
        public float? SubscriptionRevenueFluct { get; set; } = 0;

        public int Subscriber { get; set; } = 0;
        public float? SubscriberFluct { get; set; } = 0;

        public int NewSubscriber { get; set; } = 0;
        public float? NewSubscriberFluct { get; set; } = 0;

        public int QuitSubscriber { get; set; } = 0;
        public float? QuitSubscriberFluct { get; set; } = 0;

        public float? SubscriberBySubscriptionsFluct { get; set; } = 0;

        public List<Subscriber> RecentSubscribers { get; set; } = [];
        public List<SubscriberBySubscription> SubscriberBySubscriptions { get; set; } = [];
        public MostPopularTier MostPopularTier { get; set; } = new();
    }

    public class Subscriber
    {
        public UserMinimalDTO User { get; set; } = null!;
        public string SubscriptionName { get; set; } = null!;
    }
    public class MostPopularTier : SubscriberBySubscription
    {
        public float Percentage { get; set; } = 0;
    }
    public class SubscriberBySubscription
    {
        public string SubscriptionName { get; set; } = null!;
        public int NumberOfSubscriber { get; set; } = 0;
        //public float? NumberOfSubscriberFluct { get; set; }
    }
}
