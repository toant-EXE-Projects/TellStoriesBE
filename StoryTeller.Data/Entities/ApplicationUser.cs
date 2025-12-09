using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? UserType { get; set; }
        public string? Status { get; set; }
        public string? AvatarUrl { get; set; }
        [StringLength(50, ErrorMessage = "Display name cannot exceed 50 characters.")]
        public string? DisplayName { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public int LoginStreak { get; set; } = 0;
        public DateTime? DOB { get; set; }
        public DateTime? DeletionDate { get; set; }
        public Guid? ActiveSubscriptionId { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public UserSubscription? ActiveSubscription { get; set; }
        public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
        public UserWallet Wallet { get; set; } = new();

    }
}
