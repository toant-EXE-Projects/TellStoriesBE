using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<SubscriptionDTO> GetByIdAsync(Guid id);
        Task<List<SubscriptionDTO>> GetAllActiveAsync(SubscriptionPurchaseMethod? method = null, CancellationToken ct = default);
        Task<List<SubscriptionDTO>> GetAllByAdminAsync();
        Task<UserSubscription?> GetUserActiveSubscriptionAsync(string userId, CancellationToken ct = default);
        Task<UserSubscriptionDTO?> GetUserActiveSubscriptionDTOAsync(string userId, CancellationToken ct = default);
        Task<bool> CancelUserSubscription(ApplicationUser user, CancellationToken ct = default);
        Task<SubscriptionDTO> CreateAsync(SubscriptionCreateRequest subscriptionDTO, ApplicationUser user);
        Task<SubscriptionDTO> UpdateAsync(SubscriptionUpdateRequest subscriptionDTO, ApplicationUser user);
        Task<bool> SoftDelete(Guid id, ApplicationUser user);
        Task<bool> HardDelete(Guid id);
        Task<SubscriptionDashboardResponse> SubscriptionDashboard();
        Task<bool> RedeemWithPointsAsync(Guid subscriptionId, ApplicationUser user, CancellationToken ct = default);
        Task<bool> SubscribeOrExtendAsync(Guid subscriptionId, ApplicationUser user, CancellationToken ct = default);
        Task<int> MarkExpiredSubscriptionsAsync(CancellationToken ct = default);
        Task<int> NotifyUpcomingExpirationsAsync(int daysBeforeExpiration, CancellationToken ct = default);
        Task<List<UserSubscriptionDTO>> UpcomingExpirationsAsync(int daysBeforeExpiration, CancellationToken ct = default);
        Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetSubscribers(int page = 1, int pageSize = 10);
        Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetNewSubscribers(int page = 1, int pageSize = 10);
        Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetQuitSubscribers(int page = 1, int pageSize = 10);
        Task<List<PaginatedResult<SubscriptionDetailResponse>>> SubscriptionDashboardGetSubscribersBySubscription(int page = 1, int pageSize = 10);
        Task<PaginatedResult<Subscriber>> SubscriptionDashboardGetRecentSubscribers(int period, int page = 1, int pageSize = 10);
        Task<PaginatedResult<SubscriptionDetailResponse>> QuerySubscribers(string query, int page = 1, int pageSize = 10);
    }
}
