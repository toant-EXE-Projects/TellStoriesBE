using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.Services.Implementations
{
    public class UserWalletService : IUserWalletService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UserWalletService(IUnitOfWork uow, IMapper mapper, ILoggerService logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserWalletDTO> GetOrCreateUserWalletDTO(string userId, ApplicationUser user, CancellationToken ct = default)
        {
            var wal = await _uow.UserWallets.GetUserWallet(userId, ct);
            if (wal == null)
            {

                wal = new UserWallet { UserId = userId };
                await _uow.UserWallets.CreateAsync(wal, user, ct: ct);
                await _uow.SaveChangesAsync(ct);
            }

            var mapped = _mapper.Map<UserWalletDTO>(wal);
            return mapped;
        }

        public async Task<UserWallet> GetOrCreateUserWallet(string userId, ApplicationUser user, CancellationToken ct = default)
        {
            var validUser = await _uow.Users.GetByIdAsync(userId);
            if (validUser == null) throw new NotFoundException("User not found");

            var wal = await _uow.UserWallets.GetUserWallet(userId, ct);
            if (wal == null)
            {
                wal = new UserWallet { UserId = userId };
                await _uow.UserWallets.CreateAsync(wal, user, ct: ct);
                await _uow.SaveChangesAsync(ct);
            }

            return wal;
        }

        public async Task<UserWalletDTO> AddBalanceAsync(string userId, int amount, ApplicationUser? user, string? reason = null, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");

            var oldBalance = wallet.Balance;
            wallet.Add(amount);

            var transaction = new UserWalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                BalanceBefore = oldBalance,
                BalanceAfter = wallet.Balance,
                Type = WalletTransactionType.Credit,
                Description = reason ?? StringConstants.UserWalletTransaction_Credit_Desc,
                PerformedByUserId = user?.Id
            };

            await _uow.UserWalletTransactions.CreateAsync(transaction, user, ct);

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }

        public async Task<UserWalletDTO> SubtractBalanceAsync(string userId, int amount, ApplicationUser? user, string? reason = null, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");

            var oldBalance = wallet.Balance;
            wallet.Subtract(amount);

            var transaction = new UserWalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                BalanceBefore = oldBalance,
                BalanceAfter = wallet.Balance,
                Type = WalletTransactionType.Debit,
                Description = reason ?? StringConstants.UserWalletTransaction_Debit_Desc,
                PerformedByUserId = user?.Id
            };

            await _uow.UserWalletTransactions.CreateAsync(transaction, user, ct);

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }

        public async Task<UserWalletDTO> SetBalanceAsync(string userId, int newBalance, ApplicationUser? user, string? reason = null, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");

            var oldBalance = wallet.Balance;
            wallet.SetBalance(newBalance);

            var difference = newBalance - oldBalance;

            var transaction = new UserWalletTransaction
            {
                WalletId = wallet.Id,
                Amount = difference,
                BalanceBefore = oldBalance,
                BalanceAfter = newBalance,
                Type = WalletTransactionType.Adjustment,
                Description = reason ?? StringConstants.UserWalletTransaction_Set_Desc,
                PerformedByUserId = user?.Id
            };

            await _uow.UserWalletTransactions.CreateAsync(transaction, user, ct);

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }

        public async Task<UserWalletDTO> LockWalletAsync(string userId, ApplicationUser? user, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");

            wallet.Lock();

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }

        public async Task<UserWalletDTO> UnlockWalletAsync(string userId, ApplicationUser? user, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");

            wallet.Unlock();

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }

        public async Task<UserWalletDTO> ToggleWalletLockAsync(string userId, ApplicationUser? user, CancellationToken ct = default)
        {
            var wallet = await GetOrCreateUserWallet(userId, user, ct)
                         ?? throw new NotFoundException($"Wallet for user {userId} not found.");
            if (wallet.IsLocked)
                wallet.Unlock();
            else 
                wallet.Lock();

            await _uow.UserWallets.UpdateAsync(wallet, user, ct: ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserWalletDTO>(wallet);
        }
    }
}
