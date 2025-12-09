using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class SubscriptionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; } = 0;
        public string Type { get; set; } = null!;
        public int RewardPoints { get; set; } = 0;

        public int? DurationDays { get; set; }
        public bool? IsActive { get; set; }

        public int? PointsCost { get; set; }
        public SubscriptionPurchaseMethod PurchaseMethod { get; set; }
    }
}
