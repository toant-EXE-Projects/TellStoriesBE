using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Interfaces
{
    public interface IUserWalletService
    {
        Task<UserWallet> GetOrCreateUserWallet(string userId, ApplicationUser user, CancellationToken ct = default);
        Task<UserWalletDTO> GetOrCreateUserWalletDTO(string userId, ApplicationUser user, CancellationToken ct = default);
        Task<UserWalletDTO> AddBalanceAsync(string userId, int amount, ApplicationUser? user, string? reason = null, CancellationToken ct = default);
        Task<UserWalletDTO> SubtractBalanceAsync(string userId, int amount, ApplicationUser? user, string? reason = null, CancellationToken ct = default);
        Task<UserWalletDTO> SetBalanceAsync(string userId, int newBalance, ApplicationUser? user, string? reason = null, CancellationToken ct = default);
        Task<UserWalletDTO> LockWalletAsync(string userId, ApplicationUser? user, CancellationToken ct = default);
        Task<UserWalletDTO> UnlockWalletAsync(string userId, ApplicationUser? user, CancellationToken ct = default);
        Task<UserWalletDTO> ToggleWalletLockAsync(string userId, ApplicationUser? user, CancellationToken ct = default);
    }
}
