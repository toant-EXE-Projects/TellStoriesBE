using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEmailService _emailService;
        private readonly IUserWalletService _userWalletService;
        private readonly ILoggerService _logger;

        public SubscriptionService(
            IUnitOfWork uow, 
            IMapper mapper, 
            IDateTimeProvider dateTimeProvider, 
            IEmailService emailService, 
            IUserWalletService userWalletService,
            ILoggerService loggerService
        )
        {
            _uow = uow;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _emailService = emailService;
            _userWalletService = userWalletService;
            _logger = loggerService;
        }

        public async Task<UserSubscription?> GetUserActiveSubscriptionAsync(string userId, CancellationToken ct = default)
        {
            var sub = await _uow.UserSubscriptions.GetUserActiveSubscription(userId, ct);
            return sub;
        }

        public async Task<bool> CancelUserSubscription(ApplicationUser user, CancellationToken ct = default)
        {
            var sub = await GetUserActiveSubscriptionAsync(user.Id, ct);

            if (sub == null) return false;

            if (sub.Status == SubscriptionConstants.StatusCancelled)
                return false;

            if (sub.Status != SubscriptionConstants.StatusActive)
                return false;

            sub.Status = SubscriptionConstants.StatusCancelled;

            await _uow.UserSubscriptions.UpdateAsync(sub, user, ct);
            try
            {
                await _uow.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<UserSubscriptionDTO?> GetUserActiveSubscriptionDTOAsync(string userId, CancellationToken ct = default)
        {
            var sub = await _uow.UserSubscriptions.GetUserActiveSubscription(userId, ct);

            return _mapper.Map<UserSubscriptionDTO>(sub);
        }

        public async Task<int> MarkExpiredSubscriptionsAsync(CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();

            var expired = await _uow.UserSubscriptions
                .FindAsync(s => s.EndDate.HasValue &&
                                s.EndDate < now &&
                                (s.Status == null || s.Status == SubscriptionConstants.StatusActive), ct);

            foreach (var sub in expired)
            {
                sub.Status = SubscriptionConstants.StatusExpired;
            }

            return await _uow.SaveChangesAsync(ct);
        }

        public async Task<List<UserSubscriptionDTO>> UpcomingExpirationsAsync(int daysBeforeExpiration, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime().Date;
            var maxDate = now.AddDays(daysBeforeExpiration);

            var subscriptions = await _uow.UserSubscriptions.FindAsync(
                s => s.EndDate.HasValue &&
                     s.EndDate.Value.Date >= now &&
                     s.EndDate.Value.Date <= maxDate &&
                     (s.Status == null || s.Status == SubscriptionConstants.StatusActive),
                ct);

            return _mapper.Map<List<UserSubscriptionDTO>>(subscriptions);
        }

        public async Task<int> NotifyUpcomingExpirationsAsync(int daysBeforeExpiration, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime().Date;
            var maxDate = now.AddDays(daysBeforeExpiration);

            var subs = await _uow.UserSubscriptions.GetExpiringSubscriptionsAsync(now, maxDate, ct);

            int emailCount = 0;

            foreach (var userSub in subs)
            {
                var user = await _uow.Users.GetByIdAsync(userSub.UserId);
                if (user == null) continue;
                if (string.IsNullOrWhiteSpace(user.Email)) continue;

                if (userSub.EndDate.HasValue)
                {
                    try
                    {
                        var formattedEndDate = _dateTimeProvider.FormatDateTime(userSub.EndDate.Value);
                        var emailSubscriptionReminderRequest = new EmailSubscriptionReminderRequest
                        {
                            Recipient = user.Email,
                            DisplayName = user.DisplayName,
                            SubscriptionName = userSub.Subscription.Name,
                            SubscriptionExpiry = formattedEndDate,
                        };

                        await _emailService.SendSubscriptionReminderEmailAsync(
                            emailSubscriptionReminderRequest
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Couldn't send subscription reminder email. Exception: {ex.Message}");
                        continue;
                    }
                }

                userSub.HasNotified = true;
                Interlocked.Increment(ref emailCount);
            }

            await _uow.SaveChangesAsync(ct);
            return emailCount;
        }

        public async Task<bool> RedeemWithPointsAsync(Guid subscriptionId, ApplicationUser user, CancellationToken ct = default)
        {
            var subscription = await _uow.Subscriptions.GetByIdAsync(subscriptionId, ct)
                                ?? throw new NotFoundException("Subscription not found");

            if (subscription.IsDeleted || !subscription.IsCurrentlyActive) 
                throw new InvalidOperationException($"Subscription id: {subscriptionId} is no longer available.");

            if (
                subscription.PurchaseMethod != SubscriptionPurchaseMethod.PointsOnly 
                || subscription.PointsCost is null
                || subscription.PointsCost <= 0
            )
                throw new InvalidOperationException("This subscription cannot be redeemed with points.");

            var wallet = await _userWalletService.GetOrCreateUserWallet(user.Id, user, ct)
                         ?? throw new NotFoundException("User wallet not found.");

            if (wallet.Balance < subscription.PointsCost.Value)
                throw new InvalidOperationException("Insufficient points to redeem this subscription.");

            await _userWalletService.SubtractBalanceAsync(user.Id, subscription.PointsCost.Value, user, 
                reason: StringConstants.UserWalletTransaction_Redeem_Subscription, ct: ct);

            await SubscribeOrExtendAsync(subscriptionId, user, ct);

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> SubscribeOrExtendAsync(Guid subscriptionId, ApplicationUser user, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();
            var newSub = await _uow.Subscriptions.GetByIdAsync(subscriptionId, ct);
            if (newSub == null)
                throw new NotFoundException("Subscription not found");

            var userSub = await GetUserActiveSubscriptionAsync(user.Id, ct);

            var endDate = now.AddDays(newSub.DurationDays);

            // Existing Subscription
            if (userSub != null)
            {
                var currentSub = await _uow.Subscriptions.GetByIdAsync(userSub.SubscriptionId, ct);
                if (currentSub == null)
                    throw new InvalidOperationException("Current subscription record is invalid.");

                // Check if the new subscription is more expensive
                if (newSub.Price > currentSub.Price)
                {
                    userSub.Status = SubscriptionConstants.StatusExpired;
                    userSub.AutoRenew = false;
                    await _uow.UserSubscriptions.UpdateAsync(userSub, user, ct);

                    var prorateEndDate = userSub.EndDate?.AddDays(newSub.DurationDays) ?? now.AddDays(newSub.DurationDays);
                    var newUserSub = new UserSubscription
                    {
                        UserId = user.Id,
                        SubscriptionId = newSub.Id,
                        StartDate = now,
                        EndDate = prorateEndDate,
                        OriginalEndDate = userSub.OriginalEndDate,
                        RenewalDate = now,
                        Status = SubscriptionConstants.StatusActive,
                        HasNotified = false,
                        AutoRenew = false
                    };

                    await _uow.UserSubscriptions.CreateAsync(newUserSub, user, ct);
                }
                else
                {
                    // Extend current subscription
                    userSub.EndDate = userSub.EndDate?.AddDays(newSub.DurationDays);
                    userSub.RenewalDate = now;
                    userSub.OriginalEndDate ??= userSub.EndDate;
                    userSub.HasNotified = false;
                }

                await _uow.UserSubscriptions.UpdateAsync(userSub, user, ct);
            }
            else
            {
                // First-time subscription
                var newUserSub = new UserSubscription
                {
                    UserId = user.Id,
                    SubscriptionId = newSub.Id,
                    StartDate = now,
                    EndDate = endDate,
                    OriginalEndDate = endDate,
                    RenewalDate = now,
                    Status = SubscriptionConstants.StatusActive,
                    HasNotified = false,
                    AutoRenew = false // TODO: Not Today :)
                };

                await _uow.UserSubscriptions.CreateAsync(newUserSub, user, ct);
            }
            // !!DANGER!! -- ABUSE PRONE
            if (newSub.RewardPoints > 0)
            {
                var wallet = await _userWalletService.GetOrCreateUserWallet(user.Id, user, ct);
                if (wallet != null)
                {
                    await _userWalletService.AddBalanceAsync(user.Id, newSub.RewardPoints, null, 
                        reason: StringConstants.UserWalletTransaction_Subscription_Bonus, ct: ct);
                }
            }

            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<SubscriptionDTO> CreateAsync(SubscriptionCreateRequest request, ApplicationUser user)
        {
            var subscription = _mapper.Map<Subscription>(request);
            await _uow.Subscriptions.CreateAsync(subscription, user);
            await _uow.SaveChangesAsync();
            return _mapper.Map<SubscriptionDTO>(subscription);
        }

        public async Task<List<SubscriptionDTO>> GetAllActiveAsync(SubscriptionPurchaseMethod? method, CancellationToken ct = default)
        {
            var result = await _uow.Subscriptions.GetAllActiveAsync(method, ct);
            return _mapper.Map<List<SubscriptionDTO>>(result);
        }

        public async Task<List<SubscriptionDTO>> GetAllByAdminAsync()
        {
            var result = await _uow.Subscriptions.GetAllNotDeletedAsync();
            return _mapper.Map<List<SubscriptionDTO>>(result);
        }

        public async Task<SubscriptionDTO> GetByIdAsync(Guid id)
        {
            var result = await _uow.Subscriptions.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"Subscription with ID {id} not found.");

            return _mapper.Map<SubscriptionDTO>(result);
        }

        public async Task<bool> HardDelete(Guid id)
        {
            var subscription = await _uow.Subscriptions.GetByIdAsync(id);
            if (subscription == null)
                throw new NotFoundException($"Subscription with ID {id} not found.");

            _uow.Subscriptions.Remove(subscription);
            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDelete(Guid id, ApplicationUser user)
        {
            var subscription = await _uow.Subscriptions.GetByIdAsync(id);
            if (subscription == null)
                throw new NotFoundException($"Subscription with ID {id} not found.");
            subscription.IsActive = false;

            await _uow.Subscriptions.SoftRemove(subscription, user);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<SubscriptionDTO> UpdateAsync(SubscriptionUpdateRequest request, ApplicationUser user)
        {
            var subscription = await _uow.Subscriptions.GetByIdAsync(request.Id);
            if (subscription == null)
            {
                throw new NotFoundException($"Subscription with ID {request.Id} not found.");
            }
            _mapper.Map(request, subscription);
            await _uow.Subscriptions.UpdateAsync(subscription, user);
            await _uow.SaveChangesAsync();
            return _mapper.Map<SubscriptionDTO>(subscription);
        }

        public async Task<SubscriptionDashboardResponse> SubscriptionDashboard()
        {
            var result = new SubscriptionDashboardResponse();

            //var subscriptionNames = await _uow.UserSubscriptions.GetSubscriptionNamesAsync();
            var subscriptions = await _uow.Subscriptions.GetAllActiveAsync();
            var userSubscriptions = await _uow.UserSubscriptions.GetAllAsync();
            var users = await _uow.Users.GetAllAsync();

            int SubscriptionRevenueLastMonth = 0;
            int SubscriberLastMonth = 0;
            int NewSubscriberLastMonth = 0;
            int QuitSubscriberLastMonth = 0;
            List<SubscriberBySubscription> SubscriberBySubscriptionsLastMonth = [];

            foreach (var subscription in subscriptions)
            {
                result.SubscriberBySubscriptions.Add(new SubscriberBySubscription
                {
                    SubscriptionName = subscription.Name,
                    NumberOfSubscriber = 0
                });

                SubscriberBySubscriptionsLastMonth.Add(new SubscriberBySubscription
                {
                    SubscriptionName = subscription.Name,
                    NumberOfSubscriber = 0
                });
            }

            var userLookup = users.ToDictionary(u => u.Id);
            var subscriptionLookup = subscriptions.ToDictionary(s => s.Id);
            var userSubscriptionLookup = userSubscriptions.ToDictionary(us => us.Id);

            //Calculate revenue by billing record
            var billingRecords = await _uow.BillingRecords.GetAllAsync();

            foreach (var billingRecord in billingRecords)
            {
                if (billingRecord.Subscription.PurchaseMethod != Data.Enums.SubscriptionPurchaseMethod.MoneyOnly 
                    || billingRecord.Status != PaymentConst.Status_OK
                    || billingRecord.PaidAt == null)
                    continue;

                var paidDate = billingRecord.PaidAt.Value.Date;

                if (paidDate > _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date)
                    result.SubscriptionRevenue += (int)billingRecord.Total;

                if (paidDate > _dateTimeProvider.GetSystemCurrentTime().AddMonths(-2).Date)
                    SubscriptionRevenueLastMonth += (int)billingRecord.Total;
            }



            foreach (var us in userSubscriptions)
            {
                if (subscriptionLookup == null || !subscriptionLookup.TryGetValue(us.SubscriptionId, out var subscription)) continue;
                if (userLookup == null || !userLookup.TryGetValue(us.UserId, out var user)) continue;

                var startDate = us.StartDate?.Date;
                var endDate = us.EndDate?.Date;
                if (startDate == null || endDate == null) continue;

                var userHistory = userSubscriptions.Where(x => x.UserId == user.Id && x.Id != us.Id).ToList();

                if (startDate > _dateTimeProvider.GetSystemCurrentTime().AddDays(-7).Date)
                {
                    result.RecentSubscribers.Add(new Subscriber
                    {
                        User = _mapper.Map<UserMinimalDTO>(user),
                        SubscriptionName = subscription.Name
                    });
                }

                if (startDate > _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date && us.Status == SubscriptionConstants.StatusActive && !us.IsDeleted)
                {
                    if (!userHistory.Any())
                    {
                        result.NewSubscriber++;
                    }
                }
                else
                {
                    if (endDate >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date && us.Status == SubscriptionConstants.StatusActive && !us.IsDeleted)
                    {
                        SubscriberLastMonth++;
                    }
                    //nếu user này đã từng đăng ký subscription trước tháng này nhưng hiện nay đã hết hạn và không được gia hạn và đồng thời user này chưa đăng ký lại trong vòng 1 tháng này sẽ được tính là quit subscriber
                    if (endDate <= _dateTimeProvider.GetSystemCurrentTime().Date && !userHistory.Any(x => x.StartDate != null && x.StartDate.Value.Date >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date))
                        result.QuitSubscriber++;

                    if (startDate >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-2).Date)
                    {
                        if (!userHistory.Any(x => x.StartDate != null && x.StartDate.Value.Date <= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-2).Date))
                            NewSubscriberLastMonth++;
                    }

                    else if (startDate >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-3).Date)
                    {
                        if (!userHistory.Any(x => 
                            x.StartDate != null &&
                            x.StartDate.Value.Date >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-2).Date &&
                            x.StartDate.Value.Date <= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date &&
                            endDate <= _dateTimeProvider.GetSystemCurrentTime().Date
                            ))
                        {
                            QuitSubscriberLastMonth++;
                        }
                    }
                }

                if (endDate > _dateTimeProvider.GetSystemCurrentTime().Date && us.Status == SubscriptionConstants.StatusActive && !us.IsDeleted)
                {
                    result.Subscriber++;
                    var tierStat = result.SubscriberBySubscriptions.FirstOrDefault(t => t.SubscriptionName == subscription.Name);
                    //var tierStatLastMonth = SubscriberBySubscriptionsLastMonth.FirstOrDefault(t => t.SubscriptionName == subscription.Name);
                    if (tierStat != null)
                    {
                        tierStat.NumberOfSubscriber++;

                        //if (tierStatLastMonth != null)
                        //{
                        //    tierStat.NumberOfSubscriberFluct = tierStatLastMonth.NumberOfSubscriber == 0
                        //        ? null
                        //        : float.Round(100f * (tierStat.NumberOfSubscriber - tierStatLastMonth.NumberOfSubscriber) / tierStatLastMonth.NumberOfSubscriber);
                        //}

                        if (tierStat.NumberOfSubscriber >= result.MostPopularTier.NumberOfSubscriber)
                        {
                            result.MostPopularTier = new MostPopularTier
                            {
                                SubscriptionName = tierStat.SubscriptionName,
                                NumberOfSubscriber = tierStat.NumberOfSubscriber
                            };
                        }
                    }
                }
            }

            result.MostPopularTier.Percentage = result.Subscriber == 0 
                ? 0 
                : 100f * result.MostPopularTier.NumberOfSubscriber / result.Subscriber;

            result.SubscriptionRevenueFluct = SubscriptionRevenueLastMonth == 0 
                ? null 
                : float.Round(100.0f * (result.SubscriptionRevenue - SubscriptionRevenueLastMonth) / SubscriptionRevenueLastMonth, 2);

            result.SubscriberFluct = SubscriberLastMonth == 0 
                ? null 
                : float.Round(100.0f * (result.Subscriber - SubscriberLastMonth) / SubscriberLastMonth, 2);

            result.NewSubscriberFluct = NewSubscriberLastMonth == 0 
                ? null 
                : float.Round(100.0f * (result.NewSubscriber - NewSubscriberLastMonth) / NewSubscriberLastMonth, 2);

            result.QuitSubscriberFluct = QuitSubscriberLastMonth == 0 
                ? null 
                : float.Round(100.0f * (result.QuitSubscriber - QuitSubscriberLastMonth) / QuitSubscriberLastMonth, 2);

            result.SubscriberBySubscriptionsFluct = result.SubscriberFluct;
            return result;
        }

        public async Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetSubscribers(int page = 1, int pageSize = 10)
        {
            var userSubscription = await _uow.UserSubscriptions.GetAllAsync();
            List<SubscriptionDetailResponse> subscribers = [];

            foreach (var us in userSubscription)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(us.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(us.UserId);

                if (subscription == null || user == null || us.Status != SubscriptionConstants.StatusActive || us.IsDeleted)
                    continue;

                subscribers.Add(new SubscriptionDetailResponse
                {
                    UserId = user.Id,
                    User = user.DisplayName,
                    Plan = us.Subscription.Name,
                    Price = us.Subscription.Price,
                    Duration = us.Subscription.DurationDays,
                    SubscribedOn = us.StartDate,
                    OriginalEndDate = us.OriginalEndDate,
                    ExpiriesOn = us.EndDate,
                    DayRemaining = us.EndDate == null ? 0 : (us.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days
                });
            }
            return subscribers.ToPaginatedResult(page, pageSize);
        }

        public async Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetNewSubscribers(int page = 1, int pageSize = 10)
        {
            var userSubscription = await _uow.UserSubscriptions.GetAllAsync();
            var subscribers = new List<SubscriptionDetailResponse>();

            foreach (var us in userSubscription)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(us.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(us.UserId);

                if (subscription == null || user == null)
                    continue;

                var userHistory = userSubscription.Where(x => x.UserId == user.Id && x.Id != us.Id).ToList();

                if (us.StartDate != null && us.StartDate.Value.Date > _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date && !userHistory.Any() && us.Status == SubscriptionConstants.StatusActive && !us.IsDeleted)
                {
                    subscribers.Add(new SubscriptionDetailResponse
                    {
                        UserId = user.Id,
                        User = user.DisplayName,
                        Plan = us.Subscription.Name,
                        Price = us.Subscription.Price,
                        Duration = us.Subscription.DurationDays,
                        SubscribedOn = us.StartDate,
                        OriginalEndDate = us.OriginalEndDate,
                        ExpiriesOn = us.EndDate,
                        DayRemaining = us.EndDate == null ? 0 : (us.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days
                    });
                }
            }
            return subscribers.ToPaginatedResult(page, pageSize);
        }

        public async Task<PaginatedResult<SubscriptionDetailResponse>> SubscriptionDashboardGetQuitSubscribers(int page = 1, int pageSize = 10)
        {
            var userSubscription = await _uow.UserSubscriptions.GetAllAsync();
            var subscribers = new List<SubscriptionDetailResponse>();

            foreach (var us in userSubscription)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(us.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(us.UserId);

                if (subscription == null || user == null)
                    continue;

                var userHistory = userSubscription.Where(x => x.UserId == user.Id && x.Id != us.Id).ToList();

                if (us.StartDate != null && us.EndDate != null
                    && us.StartDate.Value.Date > _dateTimeProvider.GetSystemCurrentTime().AddMonths(-2).Date 
                    && !userHistory.Any(x => x.StartDate != null && x.StartDate.Value.Date >= _dateTimeProvider.GetSystemCurrentTime().AddMonths(-1).Date) 
                    && us.EndDate.Value.Date <= _dateTimeProvider.GetSystemCurrentTime().Date)
                {
                    subscribers.Add(new SubscriptionDetailResponse
                    {
                        UserId = user.Id,
                        User = user.DisplayName,
                        Plan = us.Subscription.Name,
                        Price = us.Subscription.Price,
                        Duration = us.Subscription.DurationDays,
                        SubscribedOn = us.StartDate,
                        OriginalEndDate = us.OriginalEndDate,
                        ExpiriesOn = us.EndDate,
                        DayRemaining = us.EndDate == null ? 0 : (us.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days
                    });
                }
            }
            return subscribers.ToPaginatedResult(page, pageSize);
        }

        public async Task<List<PaginatedResult<SubscriptionDetailResponse>>> SubscriptionDashboardGetSubscribersBySubscription(int page = 1, int pageSize = 10)
        {
            var subscriptions = await _uow.Subscriptions.GetAllActiveAsync();
            var userSubscription = (await _uow.UserSubscriptions.GetAllAsync());
            List<List<SubscriptionDetailResponse>> subscribers = [];
            List<PaginatedResult<SubscriptionDetailResponse>> result = [];

            foreach (var subscription in subscriptions)
            {
                subscribers.Add([]);
            }
            foreach (var us in userSubscription)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(us.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(us.UserId);

                if (subscription == null || user == null || us.Status != SubscriptionConstants.StatusActive || us.IsDeleted)
                    continue;

                for (int index = 0; index < subscriptions.Count; index++)
                    if (subscription.Id == subscriptions[index].Id)
                        subscribers[index].Add(new SubscriptionDetailResponse
                        {
                            UserId = user.Id,
                            User = user.DisplayName,
                            Plan = us.Subscription.Name,
                            Price = us.Subscription.Price,
                            Duration = us.Subscription.DurationDays,
                            SubscribedOn = us.StartDate,
                            OriginalEndDate = us.OriginalEndDate,
                            ExpiriesOn = us.EndDate,
                            DayRemaining = us.EndDate == null ? 0 : (us.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days
                        });
            }
            foreach (var item in subscribers)
            {
                result.Add(item.ToPaginatedResult(page, pageSize));
            }
            return result;
        }

        public async Task<PaginatedResult<Subscriber>> SubscriptionDashboardGetRecentSubscribers(int period, int page = 1, int pageSize = 10)
        {
            var userSubscription = await _uow.UserSubscriptions.GetAllAsync();
            var subscribers = new List<Subscriber>();
            foreach (var us in userSubscription)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(us.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(us.UserId);
                var periodDate = _dateTimeProvider.GetSystemCurrentTime().Date.AddDays(-(period-1));
                if (subscription == null 
                    || user == null 
                    || us.Status != SubscriptionConstants.StatusActive 
                    || us.IsDeleted 
                    || us.StartDate < periodDate
                    )
                    continue;

                subscribers.Add(new Subscriber
                {
                    User = _mapper.Map<UserMinimalDTO>(user),
                    SubscriptionName = subscription.Name
                });
            }
            return subscribers.ToPaginatedResult(page, pageSize);
        }

        public async Task<PaginatedResult<SubscriptionDetailResponse>> QuerySubscribers(string query, int page = 1, int pageSize = 10)
        {
            var result = new List<SubscriptionDetailResponse>();
            var queryResult = await _uow.UserSubscriptions.SearchSubscriberQuery(query);
            foreach (var item in queryResult)
            {
                var subscription = await _uow.Subscriptions.GetByIdAsync(item.SubscriptionId);
                var user = await _uow.Users.GetByIdAsync(item.UserId);

                if (subscription == null
                    || user == null
                    || item.Status != SubscriptionConstants.StatusActive
                    || item.IsDeleted
                    )
                    continue;

                result.Add(new SubscriptionDetailResponse
                {
                    UserId = user.Id,
                    User = user.DisplayName,
                    Plan = item.Subscription.Name,
                    Price = item.Subscription.Price,
                    Duration = item.Subscription.DurationDays,
                    SubscribedOn = item.StartDate,
                    OriginalEndDate = item.OriginalEndDate,
                    ExpiriesOn = item.EndDate,
                    DayRemaining = item.EndDate == null ? 0 : (item.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days
                });
            }
            return result.ToPaginatedResult(page, pageSize);
        }
    }
}
