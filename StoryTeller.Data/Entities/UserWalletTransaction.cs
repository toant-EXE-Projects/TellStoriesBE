using StoryTeller.Data.Enums;

namespace StoryTeller.Data.Entities
{
    public class UserWalletTransaction : BaseEntity
    {
        public Guid WalletId { get; set; }
        public UserWallet Wallet { get; set; } = null!;

        public int Amount { get; set; }
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }

        public WalletTransactionType Type { get; set; }
        public string? Description { get; set; }

        public string? ReferenceId { get; set; } // for linking to a StoryId, OrderId, etc.
        public string? PerformedByUserId { get; set; } // If admin or system did it
        public ApplicationUser? PerformedByUser { get; set; }

    }
}
