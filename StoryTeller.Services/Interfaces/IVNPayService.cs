using StoryTeller.Data.Entities;
using VNPAY.NET;
using VNPAY.NET.Models;

namespace StoryTeller.Services.Interfaces
{
    public interface IVNPayService
    {
        public IVnpay Vnpay { get; }
        public Task<string> CreateRequestUrl(string clientIp, Guid subscriptionId, ApplicationUser user, CancellationToken ct = default);
        public Task<bool> CheckOrderAndSubscribeOrExtendAsync(PaymentResult result, CancellationToken ct = default);
    }
}
