using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using StoryTeller.Services.Utils;

namespace StoryTeller.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUserWalletService _userWalletService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISystemConfigurationService _systemConfigService;

        public UserService(
            IUnitOfWork uow, 
            IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            IDateTimeProvider dateTimeProvider, 
            IUserWalletService userWalletService,
            RoleManager<IdentityRole> roleManager,
            ISystemConfigurationService systemConfigService
            )
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
            _dateTimeProvider = dateTimeProvider;
            _userWalletService = userWalletService;
            _roleManager = roleManager;
            _systemConfigService = systemConfigService;
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var users = await _uow.Users.GetAllAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetByIdAsync(string id)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return null;
            user.ActiveSubscription = await _uow.UserSubscriptions.GetUserActiveSubscription(id, default);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> GetByEmailAsync(string email)
        {
            var user = await _uow.Users.FindByEmailAsync(email);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> CreateAsync(UserCreateRequest request)
        {
            var user = _mapper.Map<ApplicationUser>(request);
            user.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
            user.UpdatedDate = _dateTimeProvider.GetSystemCurrentTime();
            user.UserName = request.Email;

            var isRoleExist = await _roleManager.RoleExistsAsync(request.UserType);
            if (!isRoleExist)
            {
                throw new ArgumentException($"Could not find role {request.UserType}");
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) { throw new Exception(result.Errors.FirstOrDefault()?.Description); }
            
            await _userManager.AddToRoleAsync(user, request.UserType);
            await _uow.SaveChangesAsync();
            
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> UpdateAsync(string id, UserUpdateRequest request, ApplicationUser currentUser)
        {
            var targetUser = await _uow.Users.GetByIdAsync(id);
            if (targetUser == null) return false;

            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var targetUserRole = targetUser.UserType;
            if (targetUser.UserType == Roles.Admin && !currentUserRoles.Contains(Roles.Admin))
            {
                throw new UnauthorizedAccessException("You are not allowed to update Admin accounts.");
            }

            if (targetUserRole == Roles.Admin)
            {
                var usersInAdminRole = await _userManager.GetUsersInRoleAsync(Roles.Admin);
                var adminCount = usersInAdminRole.Count;
                if (adminCount <= SystemConst.Min_Admins)
                {
                    if (request.UserType != null && request.UserType != targetUserRole)
                    {
                        throw new InvalidOperationException("Cannot change role of the last Admin(s).");
                    }
                    if (request.Status != null && request.Status != targetUser.Status)
                    {
                        throw new InvalidOperationException("Cannot change status of the last Admin(s).");
                    }
                }
            }

            targetUser.Email = request.Email == null ? targetUser.Email : request.Email;
            targetUser.DisplayName = request.DisplayName == null ? targetUser.DisplayName : request.DisplayName;
            targetUser.AvatarUrl = request.AvatarUrl == null ? targetUser.AvatarUrl : request.AvatarUrl;
            targetUser.UserType = request.UserType == null ? targetUser.UserType : request.UserType;
            targetUser.Status = request.Status == null ? targetUser.Status : request.Status;
            targetUser.PhoneNumber = request.PhoneNumber == null ? targetUser.PhoneNumber : request.PhoneNumber;
            targetUser.DOB = request.DOB == null ? targetUser.DOB : request.DOB;

            targetUser.NormalizedEmail = targetUser.Email?.ToUpper();
            targetUser.UpdatedDate = _dateTimeProvider.GetSystemCurrentTime();

            _uow.Users.Update(targetUser);

            if (request.UserType != null && targetUserRole != null)
            {
                await _userManager.RemoveFromRoleAsync(targetUser, targetUserRole);
                await _userManager.AddToRoleAsync(targetUser, request.UserType);
            }

            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id, ApplicationUser currentUser)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return false;
            
            // Dont allow the system admins to canibalize themselves
            var targetIsAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
            if (targetIsAdmin)
            {
                var usersInAdminRole = await _userManager.GetUsersInRoleAsync(Roles.Admin);
                var adminCount = usersInAdminRole.Count;

                if (adminCount <= SystemConst.Min_Admins)
                {
                    throw new InvalidOperationException("Cannot delete the last Admin(s)");
                }
                throw new InvalidOperationException("Admins cannot delete their own account.");
            }

            return await SoftDeleteUserAsync(user);
        }

        private async Task<bool> SoftDeleteUserAsync(ApplicationUser user)
        {
            user.DisplayName = UserStatusConstants.DeletedName;
            user.UserName = string.Format(UserStatusConstants.DeletedUserName, user.Id);
            user.NormalizedUserName = null;
            user.Status = UserStatusConstants.Deleted;
            user.Email = string.Format(UserStatusConstants.DeletedEmail, user.Id);
            user.NormalizedEmail = null;
            user.PhoneNumber = null;
            user.PasswordHash = null;
            user.AvatarUrl = null;
            user.UserType = null;
            user.DOB = null;
            user.DeletionDate = _dateTimeProvider.GetSystemCurrentTime();
            user.IsDeleted = true;

            user.ActiveSubscriptionId = null;
            user.ActiveSubscription = null;

            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteOwnAccountAsync(string userId, string currentPassword, ApplicationUser currentUser)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Validate password
            var passwordValid = await _userManager.CheckPasswordAsync(user, currentPassword);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid password.");

            var result = await DeleteAsync(userId, currentUser);
            return result;
        }

        /// <summary>
        /// Updates Last Login + Login Streak
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns>Consecutive login streak</returns>
        public async Task<int> UpdateLastLoginAsync(string userId, CancellationToken ct = default)
        {
            var streakCount = 1;
            var now = _dateTimeProvider.GetSystemCurrentTime();
            var today = now.Date;
            var yesterday = today.AddDays(-1);

            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null) return 0;

            var lastLoginDate = user.LastLogin?.Date;

            user.LastLogin = now;

            if (lastLoginDate != today)
            {
                if (lastLoginDate == yesterday)
                {
                    user.LoginStreak = streakCount + 1;
                }
                else
                {
                    user.LoginStreak = streakCount; // Reset streak
                }

                var sysConfig_Login_DailyReward_Points = await _systemConfigService.GetIntAsync(
                    SystemConst.Keys.Login_DailyReward_Points,
                    SystemConst.Values.Login_DailyReward_Points,
                    ct
                );
                var pointsToAward = DailyStreakHelper.CalculateDailyStreakPoints(user.LoginStreak, sysConfig_Login_DailyReward_Points);
                if (pointsToAward > 0)
                {
                    await _userWalletService.AddBalanceAsync(user.Id, pointsToAward, null, 
                        reason: StringConstants.UserWalletTransaction_Daily_login_Reward, ct: ct);
                }
            }

            _uow.Users.Update(user);

            try
            {
                await _uow.SaveChangesAsync(ct);
                return user.LoginStreak;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var res = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!res.Succeeded)
            {
                var errorDescriptions = string.Join("\n", res.Errors.Select(e => e.Description));
                return IdentityResult.Failed(new IdentityError { Description = errorDescriptions });
            }

            if (currentPassword.Equals(newPassword))
                return IdentityResult.Failed(new IdentityError { Description = "Current password cannot be the same as new password." });

            await _uow.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<List<UserDTO>> SearchAsync(string searchValue)
        {
            var users = await _uow.Users.GetAllAsync();
            List<ApplicationUser> result = [];
            foreach (var user in users)
            {
                if (user.DisplayName != null && user.Email != null && (user.DisplayName.Contains(searchValue) || user.Email.Contains(searchValue)))
                {
                    result.Add(user);
                }
            }
            return _mapper.Map<List<UserDTO>>(result);
        }

        public async Task<bool> ChangeRoleAsync(string id, string role)
        {
            var user = await _uow.Users.GetByIdAsync(id);
            if (user == null) return false;

            if (role != null && user.UserType != null && !user.UserType.Equals(role))
            {
                await _userManager.RemoveFromRoleAsync(user, user.UserType);
                user.UserType = role;
                await _userManager.AddToRoleAsync(user, role);
            }

            await _uow.SaveChangesAsync();
            return true;
        }

        //da thong ke statistic trong vong 7 ngay voi logic moi, h can tim cach thong ke trong nhung ngay con lai
        public async Task<DashboardResponse> Dashboard(int statisticPeriod)
        {
            DashboardResponse result = new();

            var users = await _uow.Users.GetActiveUserAsync();
            var stories = await _uow.Stories.GetAllAsync();

            int newAccountLastWeek = 0;
            int activeAccountPrev = 0;
            int publishedStoriesLastWeek = 0;
            int storiesViewsLastWeek = 0;

            var currentDate = _dateTimeProvider.GetSystemCurrentTime().Date;
            for (DateTime date = currentDate.AddDays(-statisticPeriod) ; date <= currentDate; date = date.AddDays(1)) 
            {
                result.Statistics.Add(new Statistic
                {
                    Date = date,
                    NewAccount = 0,
                    PublishedStories = 0
                });
            }
            var statisticLastIndex = result.Statistics.Count - 1;
            foreach (var user in users)
            {
                if (user.CreatedDate != null)
                {
                    var systemDate = _dateTimeProvider.GetSystemCurrentTime().Date;
                    var daysAgo = (systemDate - user.CreatedDate.Value.Date).Days;

                    if (daysAgo >= 0 && daysAgo <= 7)
                    {
                        result.NewAccount++;
                    }
                    if (daysAgo <= 14)
                    {
                        newAccountLastWeek++;
                    }

                    if (statisticLastIndex - daysAgo >= 0)
                        result.Statistics[statisticLastIndex - daysAgo].NewAccount++;
                }

                var logs = (await _uow.ActivityLogs.GetByUserId(user.Id, DateTime.MinValue, currentDate))!.OrderByDescending(al => al.Timestamp);
                bool isActiveOrNot = false; 
                foreach (var log in logs)
                {
                    var systemDate = _dateTimeProvider.GetSystemCurrentTime().Date;
                    var daysAgo = (systemDate - log.CreatedDate.Date).Days;

                    // check log for published stories
                    if (string.Equals(log.Action.Trim(), ActivityLogConst.Action.PUBLISH)
                        && string.Equals(log.TargetType.Trim(), ActivityLogConst.TargetType.STORY))
                    {
                        if (daysAgo >= 0 && daysAgo <= 7)
                        {
                            result.PublishedStories++;
                        }
                        else if (daysAgo <= 14)
                        {
                            publishedStoriesLastWeek++;
                        }

                        if (statisticLastIndex - daysAgo >= 0)
                            result.Statistics[statisticLastIndex - daysAgo].PublishedStories++;
                    }

                    // check log for story view
                    if (log.Action.Equals(ActivityLogConst.Action.VIEW, StringComparison.OrdinalIgnoreCase)
                        && log.TargetType.Equals(ActivityLogConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
                    {
                        if (daysAgo >= 0 && daysAgo <= 7)
                        {
                            result.StoriesViews++;
                        }
                        else if (daysAgo <= 14)
                        {
                            storiesViewsLastWeek++;
                        }

                        if (statisticLastIndex - daysAgo >= 0)
                            result.Statistics[statisticLastIndex - daysAgo].StoriesViews++;
                    }

                    if (log.Action.Equals(ActivityLogConst.Action.SYSTEM, StringComparison.OrdinalIgnoreCase) 
                        && log.TargetType.Equals(ActivityLogConst.TargetType.USER, StringComparison.OrdinalIgnoreCase))
                    {
                        if (statisticLastIndex - daysAgo >= 0)
                            // check log for active user
                            result.Statistics[statisticLastIndex - daysAgo].ActiveAccount++;

                        // if already confirm user is active or not then no need to check the log detail
                        if (isActiveOrNot)
                            continue;

                        if (log.Details!.Contains("đăng xuất", StringComparison.OrdinalIgnoreCase))
                        {
                            isActiveOrNot = true;
                            continue;
                        }

                        if (log.Details!.Contains("đăng nhập thành công", StringComparison.OrdinalIgnoreCase))
                        {
                            result.ActiveAccount++;
                            isActiveOrNot = true;
                        }
                    }
                }

                isActiveOrNot = false;
                var logsLast5Sec = logs.Where(l => l.Timestamp <= _dateTimeProvider.GetSystemCurrentTime().AddSeconds(-5));
                foreach (var log in logsLast5Sec)
                {
                    if (isActiveOrNot)
                        continue;

                    if (log.Details!.Contains("đăng nhập thành công", StringComparison.OrdinalIgnoreCase))
                    {
                        activeAccountPrev++;
                        isActiveOrNot = true;
                    }
                    if (log.Details!.Contains("đăng xuất", StringComparison.OrdinalIgnoreCase))
                    {
                        isActiveOrNot = true;
                    }
                }
            }

            // Change the logic to check activity log for published stories
            //foreach (var story in stories)
            //{
            //    if (story.UpdatedAt == null) continue;
            //    if (story.IsPublished && !story.IsDeleted)
            //    {
            //        var systemDate = _dateTimeProvider.GetSystemCurrentTime().Date;
            //        var daysAgo = (systemDate - story.UpdatedAt.Value.Date).Days;

            //        if (daysAgo >= 0 && daysAgo <= 7)
            //        {
            //            result.PublishedStories++;
            //        }
            //        else if (daysAgo <= 14)
            //        {
            //            publishedStoriesLastWeek++;
            //        }

            //        if (statisticLastIndex - daysAgo >= 0)
            //            result.Statistics[statisticLastIndex - daysAgo].PublishedStories++;
            //    }
            //}

            result.NewAccountFluct = newAccountLastWeek == 0 ? (result.NewAccount > 0 ? 100 : 0) 
                : float.Round(100.0f * (result.NewAccount - newAccountLastWeek) / newAccountLastWeek, 2);

            result.ActiveAccountFluct = activeAccountPrev == 0 ? (result.ActiveAccount > 0 ? 100 : 0)
                : float.Round(100.0f * (result.ActiveAccount - activeAccountPrev) / activeAccountPrev, 2);

            result.PublishedStoriesFluct = publishedStoriesLastWeek == 0 ? (result.PublishedStories > 0 ? 100 : 0)
                : float.Round(100.0f * (result.PublishedStories - publishedStoriesLastWeek) / publishedStoriesLastWeek, 2);

            result.StoriesViewsFluct = storiesViewsLastWeek == 0 ? (result.StoriesViews > 0 ? 100 : 0)
                : float.Round(100.0f * (result.StoriesViews - storiesViewsLastWeek) / storiesViewsLastWeek, 2);
            return result;
        }

        public async Task<PaginatedResult<UserDTO>> QueryUser(UserQueryRequest request)
        {
            bool asc = false;
            if (request.SortOrder == "asc")
            {
                asc = true;
            }
            else if (request.SortOrder != "desc" && request.SortOrder != null) { throw new ArgumentException("Wrong format of sortOrder (must be asc or desc)"); }

            return _mapper.Map<PaginatedResult<UserDTO>>(await _uow.Users.SearchAndFilterAsync(
                request.SearchValue, 
                request.FilterByRole, 
                request.FilterByStatus, 
                request.OrderBy, 
                asc, 
                request.PageNumber, 
                request.PageSize));
        }

        public async Task<SubscriptionDetailResponse> GetSubscriptionDetail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var userSubscription = await _uow.UserSubscriptions.GetUserActiveSubscription(userId, default);
            if (userSubscription == null)
            {
                throw new NotFoundException("User do not have a subscription activated");
            }

            var result = _mapper.Map<SubscriptionDetailResponse>(userSubscription);
            result.DayRemaining = userSubscription.EndDate == null 
                ? 0 
                : (userSubscription.EndDate.Value.Date - _dateTimeProvider.GetSystemCurrentTime().Date).Days;

            return result;
        }

        public async Task<BillingHistoryResponse> BillingHistory(string userId, bool? ascOrder, int page = 1, int pageSize = 10)
        {
            var billingRecords = await _uow.BillingRecords.GetByUserId(userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var currentSubscription = await _uow.UserSubscriptions.GetUserActiveSubscription(userId, default);
            if (currentSubscription == null)
            {
                throw new Exception("User do not have a subscription activated");
            }
            List<BillingRecordDTO> dataList = new List<BillingRecordDTO>();

            BillingHistoryResponse result = new BillingHistoryResponse
            {
                UserName = user.DisplayName,
                Email = user.Email,
                RemainSubscriptionDays = (currentSubscription.EndDate! - currentSubscription.StartDate!).Value.Days,
                
                SubscriptionName = currentSubscription.Subscription.Name,
                SubscriptionPrice = currentSubscription.Subscription.Price,
                SubscriptionDurationDays = currentSubscription.Subscription.DurationDays,
                SubscriptionEndDate = currentSubscription.EndDate
            };
            
            foreach (var billingRecord in billingRecords)
            {
                if (billingRecord.Subscription.PurchaseMethod == Data.Enums.SubscriptionPurchaseMethod.MoneyOnly && billingRecord.Status == PaymentConst.Status_OK)
                    result.TotalMoneySpent += (int)billingRecord.Total;

                dataList.Add(_mapper.Map<BillingRecordDTO>(billingRecord));
            }

            if (ascOrder == null)
            {
                result.BillingHistory = dataList.ToPaginatedResult(page, pageSize);
                return result;
            }

            // Order billing history at here but not at repo because there are some billing record have PaidAt attribute null lead to error. So I use CreatedDate as an alternative option.
            dataList = ascOrder == false 
                ? dataList.OrderByDescending(bh => bh.PaidAt).ToList()
                : dataList.OrderBy(bh => bh.PaidAt).ToList();

            result.BillingHistory = dataList.ToPaginatedResult(page, pageSize);
            return result;
        }

        public async Task<PaginatedResult<BillingRecordDTO>> BillingHistoryQuery(string? query, bool? ascOrder, int page = 1, int pageSize = 10)
        {
            var billingRecords = await _uow.BillingRecords.QueryBillingRecords(query, default);
            List<BillingRecordDTO> result = new List<BillingRecordDTO>();
            foreach (var billingRecord in billingRecords)
            {
                result.Add(_mapper.Map<BillingRecordDTO>(billingRecord));
            }

            if (ascOrder == null) return result.ToPaginatedResult(page, pageSize);

            // Order billing history at here but not at repo because there are some billing record have PaidAt attribute null lead to error. So I use CreatedDate as an alternative option.
            result = ascOrder == false
                ? result.OrderByDescending(bh => bh.PaidAt).ToList()
                : result.OrderBy(bh => bh.PaidAt).ToList();

            return result.ToPaginatedResult(page, pageSize);
        }

        public async Task<List<BillingRecordDTO>> RevenueDetail(int period)
        {
            var query = await _uow.BillingRecords.QueryBillingRecords(PaymentConst.Status_OK, default);
            var result = query
                .Where(b => b.Status == PaymentConst.Status_OK && b.PaidAt > _dateTimeProvider.GetSystemCurrentTime().AddDays(-period).Date)
                .ToList();

            return _mapper.Map<List<BillingRecordDTO>>(result);
        }
    }
}
