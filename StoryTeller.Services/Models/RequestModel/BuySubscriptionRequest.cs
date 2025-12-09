using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class BuySubscriptionRequest
    {
        public string UserId { get; set; } = null!;
        public Guid SubscriptionId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RenewalDate { get; set; }

        public bool? HasNotified { get; set; }
        public bool? AutoRenew { get; set; }
    }
}
