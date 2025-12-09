using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Configurations;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using System.Net;
using System.Net.Mail;

namespace StoryTeller.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly EmailSettings? _settings;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerService _logger;

        public EmailService(IConfiguration config, IWebHostEnvironment env, ILoggerService loggerService)
        {
            _config = config;
            _env = env;
            _logger = loggerService;

            _settings = _config.GetSection("EmailSettings").Get<EmailSettings>();
            if (_settings == null ||
                string.IsNullOrWhiteSpace(_settings.SmtpServer) ||
                _settings.Port <= 0 ||
                string.IsNullOrWhiteSpace(_settings.Username) ||
                string.IsNullOrWhiteSpace(_settings.Password) ||
                string.IsNullOrWhiteSpace(_settings.SenderEmail) ||
                string.IsNullOrWhiteSpace(_settings.SenderName) ||
                string.IsNullOrWhiteSpace(_settings.SupportEmail))
            {
                _logger.LogWarning("⚠️ Email settings missing or incomplete. EmailService will be unavailable.");
            }
        }

        private void AddRecipients(MailAddressCollection collection, string recipients)
        {
            var addresses = recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var address in addresses)
            {
                collection.Add(address.Trim());
            }
        }
        public async Task<string> GetTemplateAsync(string fileName)
        {
            var path = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, EmailTemplates.EmailTemplatePath, fileName);
            return await File.ReadAllTextAsync(path);
        }

        public async Task SendSubscriptionReminderEmailAsync(EmailSubscriptionReminderRequest request)
        {
            var body = await GetTemplateAsync(EmailTemplates.SubscriptionReminderFile);

            body = body.Replace("{{displayName}}", request.DisplayName)
                       .Replace("{{subscriptionName}}", request.SubscriptionName)
                       .Replace("{{subscriptionExpiry}}", request.SubscriptionExpiry)
                       .Replace("{{supportEmail}}", request.SupportEmail ?? _settings.SupportEmail);

            await SendEmailAsync(request.Recipient, request.Subject ?? EmailTemplates.SubscriptionSuccessSubject, body);
        }

        public async Task SendVerificationCodeEmailAsync(EmailVerificationCodeRequest request)
        {
            var body = await GetTemplateAsync(EmailTemplates.VerificationCodeFile);

            body = body.Replace("{{displayName}}", request.DisplayName)
                       .Replace("{{bodyPurpose}}", request.BodyPurpose)
                       .Replace("{{code}}", request.Code)
                       .Replace("{{supportEmail}}", request.SupportEmail ?? _settings.SupportEmail);

            await SendEmailAsync(request.Recipient, request.Subject ?? EmailTemplates.SubscriptionSuccessSubject, body);
        }

        public async Task SendSubscriptionReceiptEmailAsync(EmailSubscriptionReceiptRequest request)
        {
            var body = await GetTemplateAsync(EmailTemplates.SubscriptionSuccessBodyFile);

            body = body.Replace("{{displayName}}", request.SubscriberName)
                       .Replace("{{subscriptionName}}", request.SubscriptionName)
                       .Replace("{{subscriptionId}}", request.SubscriptionId)
                       .Replace("{{subscriberEmail}}", request.SubscriberEmail)
                       .Replace("{{transactionId}}", request.TransactionId)
                       .Replace("{{paymentId}}", request.PaymentId)
                       .Replace("{{paidAt}}", request.PaidAt)
                       .Replace("{{paymentMethod}}", request.PaymentMethod)
                       .Replace("{{total}}", request.Total)
                       .Replace("{{supportEmail}}", request.SupportEmail ?? _settings.SupportEmail);

            await SendEmailAsync(request.Recipient, request.Subject ?? EmailTemplates.SubscriptionSuccessSubject, body);
        }

        public async Task SendEmailAsync(
            string toRecipients,
            string subject,
            string htmlBody,
            string? ccRecipients = null,
            string? bccRecipients = null)
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            // Add To recipients
            AddRecipients(mail.To, toRecipients);

            // Add CC recipients (if any)
            if (!string.IsNullOrWhiteSpace(ccRecipients))
                AddRecipients(mail.CC, ccRecipients);

            // Add BCC recipients (if any)
            if (!string.IsNullOrWhiteSpace(bccRecipients))
                AddRecipients(mail.Bcc, bccRecipients);

            try
            {
                await client.SendMailAsync(mail);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP error while sending email: {smtpEx.Message}");
                _logger.LogError(smtpEx.Message, smtpEx);
            }
            catch (FormatException formatEx)
            {
                // Handle invalid email address format
                Console.WriteLine($"Invalid email format: {formatEx.Message}");
                _logger.LogError(formatEx.Message, formatEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error while sending email: {ex.Message}");
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
