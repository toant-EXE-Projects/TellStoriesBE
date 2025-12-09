using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;

namespace StoryTeller.Services.Models.DTO
{
    public class UserWalletTransactionDTO
    {
        //public Guid WalletId { get; set; }
        public UserWalletDTO Wallet { get; set; } = null!;

        public int Amount { get; set; } 
        public int BalanceBefore { get; set; }
        public int BalanceAfter { get; set; }

        public WalletTransactionType Type { get; set; }
        public string? Description { get; set; }

        public string? ReferenceId { get; set; }
        //public string? PerformedByUserId { get; set; }
        public UserMinimalDTO? PerformedByUser { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
