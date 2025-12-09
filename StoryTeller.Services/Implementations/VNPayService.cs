using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Configurations;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;

namespace StoryTeller.Services.Implementations
{
    public class VNPayService : IVNPayService
    {
        private readonly VnPaySettings _config;
        public readonly IVnpay _vnpay;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public IVnpay Vnpay => _vnpay;

        public VNPayService(
            IVnpay vnpay,
            IOptions<VnPaySettings> options,
            IDateTimeProvider dateTimeProvider,
            IUnitOfWork uow,
            ISubscriptionService subscriptionService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager
        )
        {
            _config = options.Value;
            _dateTimeProvider = dateTimeProvider;
            _vnpay = vnpay;
            _uow = uow;
            _subscriptionService = subscriptionService;
            _userManager = userManager;
            _emailService = emailService;

            _vnpay.Initialize(_config.TmnCode, _config.HashSecret, _config.BaseUrl, _config.ReturnUrl);
        }

        public async Task<string> CreateRequestUrl(string clientIp, Guid subscriptionId, ApplicationUser user, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();
            var paymentId = now.Ticks;

            var sub = await _uow.Subscriptions.GetByIdAsync(subscriptionId);
            if (sub == null) throw new NotFoundException($"Subscription id: {subscriptionId} not found");
            if (sub.IsDeleted || sub.IsActive == false) throw new InvalidOperationException($"Subscription id: {subscriptionId} is no longer available.");

            var orderDesc = string.Format(PaymentConst.Payment_Description_Builder, user.Id, user.Email, sub.Id, sub.Name);

            var billing = new BillingRecord
            {
                BillingId = paymentId.ToString(),
                UserId = user.Id,
                SubscriptionId = subscriptionId,
                Subtotal = (decimal)sub.Price,
                DiscountAmount = 0,
                PaymentMethod = PaymentConst.Method_Online,
                PaymentGateway = PaymentConst.VNPay,
                TransactionId = null,
                Status = PaymentConst.Status_Pending,
                Notes = orderDesc
            };
            await _uow.BillingRecords.CreateAsync(billing, user, ct);
            await _uow.SaveChangesAsync(ct);

            var request = new PaymentRequest
            {
                PaymentId = paymentId,
                Money = sub.Price,
                Description = orderDesc,
                IpAddress = clientIp,
                BankCode = BankCode.ANY,
                CreatedDate = _dateTimeProvider.GetSystemCurrentTime(),
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            return _vnpay.GetPaymentUrl(request);
        }

        public async Task<bool> CheckOrderAndSubscribeOrExtendAsync(PaymentResult result, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();

            var order = await _uow.BillingRecords.GetByBillingId(result.PaymentId.ToString(), ct);
            if (order == null) throw new InvalidOperationException("Invalid order details");

            order.TransactionId = result.VnpayTransactionId.ToString();
            order.Status = PaymentConst.Status_OK;
            order.PaymentMethod = result.PaymentMethod;

            order.PaymentRawData = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            order.PaidAt = now;

            await _uow.SaveChangesAsync(ct);

            var orderSubscriptionId = order.SubscriptionId;
            var orderSubscription = await _subscriptionService.GetByIdAsync(order.SubscriptionId);
            var orderUser = await _userManager.FindByIdAsync(order.UserId);
            if (orderUser == null) throw new InvalidOperationException("User Not Found.");

            var res = await _subscriptionService.SubscribeOrExtendAsync(orderSubscriptionId, orderUser, ct);

            var receiptEmailRequest = new EmailSubscriptionReceiptRequest
            {
                Recipient = orderUser.Email!,
                Subject = EmailTemplates.SubscriptionSuccessSubject,
                SubscriberName = orderUser.DisplayName,
                SubscriptionName = orderSubscription.Name,
                SubscriptionId = orderSubscription.Id.ToString(),
                SubscriberEmail = orderUser.Email!,
                TransactionId = result.VnpayTransactionId.ToString(),
                PaymentId = result.PaymentId.ToString(),
                PaidAt = _dateTimeProvider.FormatDateTime(result.Timestamp),
                PaymentMethod = result.PaymentMethod,
                Total = order.Total.ToString("C0", new CultureInfo("vi-VN"))
            }; 
            await _emailService.SendSubscriptionReceiptEmailAsync(receiptEmailRequest);
            return res;
        }

    }
}
