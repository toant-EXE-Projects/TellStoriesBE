using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toRecipients,
            string subject,
            string htmlBody,
            string? ccRecipients = null,
            string? bccRecipients = null);
        Task SendSubscriptionReceiptEmailAsync(EmailSubscriptionReceiptRequest request);
        Task SendVerificationCodeEmailAsync(EmailVerificationCodeRequest request);
        Task SendSubscriptionReminderEmailAsync(EmailSubscriptionReminderRequest request);
    }
}
