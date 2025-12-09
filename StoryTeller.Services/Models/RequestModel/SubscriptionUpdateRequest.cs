using StoryTeller.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class SubscriptionUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int? Price { get; set; }
        public string? Type { get; set; }

        public int? DurationDays { get; set; }

        public int? PointsCost { get; set; }

        public int RewardPoints { get; set; } = 0;
        public SubscriptionPurchaseMethod PurchaseMethod { get; set; }
        public bool? IsActive { get; set; }
    }
}
