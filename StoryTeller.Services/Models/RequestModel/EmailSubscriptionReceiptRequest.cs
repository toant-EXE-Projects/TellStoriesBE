namespace StoryTeller.Services.Models.RequestModel
{
    public class EmailSubscriptionReceiptRequest
    {
        public string? Subject { get; set; }
        public string? SupportEmail { get; set; }
        public string Recipient { get; set; } = null!;
        public string SubscriberName { get; set; } = null!;
        public string SubscriptionName { get; set; } = null!;
        public string SubscriptionId { get; set; } = null!;
        public string SubscriberEmail { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public string PaymentId { get; set; } = null!;
        public string PaidAt { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string Total { get; set; } = null!;

    }
}
