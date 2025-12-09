using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Mscc.GenerativeAI;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Utils;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading;
using Yarp.ReverseProxy.Utilities;

namespace StoryTeller.API.Utils
{
    public static class UsageLimitKeys
    {
        public const string IpPrefix = "ip";
        public const string UserPrefix = "user";
        public const string RoutePrefix = "route";

        public static string ForUser(string userId) => $"{UserPrefix}:{userId}";
        public static string ForUserAndRoute(string userId, string route) => $"{UserPrefix}:{userId}:{RoutePrefix}:{route}";
        public static string ForIp(string ip) => $"{IpPrefix}:{ip}";
        public static string ForIpAndRoute(string ip, string route) => $"{IpPrefix}:{ip}:{RoutePrefix}:{route}";
    }

    public interface IUsageLimiter
    {
        Task<bool> CanProceedAsync(
            HttpContext httpContext,
            int freeLimit,
            TimeSpan window,
            Func<Task<bool>> isPremiumUserCheckFunc,
            string? route = null
        );
        Task<bool> Check(
            ApplicationUser user,
            HttpContext httpContext,
            int freeLimit,
            double windowHours,
            string? route = null,
            CancellationToken ct = default
        );
    }
    /// <summary>
    /// limits api usage by ip
    /// </summary>
    /// <example>
    ///    bool canProceed = await _usageLimiter.CanProceedAsync(
    ///    HttpContext,
    ///        freeLimit: 3,
    ///        window: TimeSpan.FromDays(1),
    ///        isPremiumUserCheck: async() =>
    ///        {
    ///            if (user == null) return false;
    ///            var sub = await _subscriptionService.GetUserSubscriptionAsync(user.Id);
    ///            return sub != null;
    ///        });
    ///
    ///     if (!canProceed)
    ///         return Forbid("Free usage limit reached. Please upgrade your subscription.");
    ///
    /// </example>
    public class UsageLimiter : IUsageLimiter
    {
        private readonly IMemoryCache _cache;
        private readonly ISubscriptionService _subscriptionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

        public UsageLimiter(
            IMemoryCache cache,
            ISubscriptionService subscriptionService,
            UserManager<ApplicationUser> userManager
        )
        {
            _cache = cache;
            _subscriptionService = subscriptionService;
            _userManager = userManager;
        }

        public async Task<bool> CanProceedAsync(
            HttpContext httpContext,
            int freeLimit,
            TimeSpan window,
            Func<Task<bool>> isPremiumUserCheckFunc,
            string? route = null
        )
        {
            if (await isPremiumUserCheckFunc())
                return true;
            if (freeLimit <= 0) return false;

            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (!string.IsNullOrWhiteSpace(route))
                route = route?.Trim().ToLowerInvariant();

            string? userId = httpContext.User?.Identity?.IsAuthenticated == true
                ? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            // Key based on priority: userId > ip
            string key = !string.IsNullOrWhiteSpace(userId)
                ? (!string.IsNullOrWhiteSpace(route)
                    ? UsageLimitKeys.ForUserAndRoute(userId, route)
                    : UsageLimitKeys.ForUser(userId))
                : (!string.IsNullOrWhiteSpace(route)
                    ? UsageLimitKeys.ForIpAndRoute(ip, route)
                    : UsageLimitKeys.ForIp(ip));

            // Get or create a semaphore for this key
            var semaphore = _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();
            try
            {
                var count = _cache.TryGetValue(key, out int existingCount) ? existingCount : 0;

                if (count >= freeLimit)
                    return false;

                _cache.Set(key, count + 1, window);
                return true;
            }
            finally
            {
                semaphore.Release();
            }


            //lock (_lock)
            //{
            //    var count = _cache.Get<int>(key); // _cache.TryGetValue(key, out int existingCount) ? existingCount : 0;

            //    if (count >= freeLimit)
            //        return false;

            //    _cache.Set(key, count + 1, window);
            //    return true;
            //}
        }

        public async Task<bool> Check(
            ApplicationUser user,
            HttpContext httpContext,
            int freeLimit,
            double windowHours,
            string? route = null,
            CancellationToken ct = default
        )
        {

            bool allowed = await CanProceedAsync(
                httpContext,
                freeLimit: freeLimit,
                window: TimeSpan.FromHours(windowHours),
                isPremiumUserCheckFunc: async () =>
                {
                    if (await user.IsStaffAsync(_userManager)) return true;
                    if (user == null) return false;
                    var sub = await _subscriptionService.GetUserActiveSubscriptionAsync(user.Id, ct);
                    return sub != null;
                },
                route: route
            );

            return allowed;
        }
    }
}
