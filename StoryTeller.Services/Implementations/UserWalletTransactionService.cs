using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Implementations
{
    public class UserWalletTransactionService : IUserWalletTransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public UserWalletTransactionService(IUnitOfWork uow, IMapper mapper, ILoggerService logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResult<UserWalletTransactionDTO>> GetTransactionsAsync(
            WalletTransactionRequest query,
            CancellationToken ct = default
        )
        {
            var result = await _uow.UserWalletTransactions.GetAllTransactions(
                query.Page, query.PageSize, query.UserQuery, query.Type, query.From, query.To, ct);

            var res = _mapper.Map<PaginatedResult<UserWalletTransactionDTO>>(result);
            return res;
        }
    }
}
