using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class BillingRecordDTO
    {
        public string BillingId { get; set; } = null!;

        public string UserId { get; set; }
        public UserMinimalDTO User { get; set; } = null!;

        public Guid SubscriptionId { get; set; }
        public SubscriptionDTO Subscription { get; set; }

        public decimal Subtotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Total => Subtotal - DiscountAmount;

        public DateTime? PaidAt { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string PaymentGateway { get; set; } = null!;

        public string? TransactionId { get; set; }

        public string Status { get; set; }

        public string? InvoiceUrl { get; set; }

        public string? Notes { get; set; }
    }
}
