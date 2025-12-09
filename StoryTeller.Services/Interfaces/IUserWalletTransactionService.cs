using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Interfaces
{
    public interface IUserWalletTransactionService
    {
        Task<PaginatedResult<UserWalletTransactionDTO>> GetTransactionsAsync(
            WalletTransactionRequest query,
            CancellationToken ct = default
        );
    }
}
