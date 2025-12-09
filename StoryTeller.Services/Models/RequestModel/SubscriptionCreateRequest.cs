using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class SubscriptionCreateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Type { get; set; }

        public int? DurationDays { get; set; }
        public int? PointsCost { get; set; }
        public int RewardPoints { get; set; } = 0;
        public SubscriptionPurchaseMethod PurchaseMethod { get; set; }
        public bool? IsActive { get; set; }
    }
}
