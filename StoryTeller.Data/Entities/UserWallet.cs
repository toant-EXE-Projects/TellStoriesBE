using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class UserWallet : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int Balance { get; private set; } = 0;
        public bool IsLocked { get; private set; } = false;

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public ICollection<UserWalletTransaction> Transactions { get; set; } = new List<UserWalletTransaction>();


        public void Lock() => IsLocked = true;
        public void Unlock() => IsLocked = false;

        public void Add(int amount)
        {
            EnsureNotLocked();
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            Balance += amount;
        }

        public void Subtract(int amount)
        {
            EnsureNotLocked();
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            if (Balance < amount) throw new InvalidOperationException("Insufficient funds.");
            Balance -= amount;
        }

        public void SetBalance(int newBalance)
        {
            EnsureNotLocked();
            if (newBalance < 0) throw new ArgumentOutOfRangeException(nameof(newBalance), "Balance cannot be negative.");
            Balance = newBalance;
        }

        private void EnsureNotLocked()
        {
            if (IsLocked) throw new InvalidOperationException("Wallet is locked.");
        }
    }
}
