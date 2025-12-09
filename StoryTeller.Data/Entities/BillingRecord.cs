using StoryTeller.Data.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryTeller.Data.Entities
{
    //https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
    public class BillingRecord : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string BillingId { get; set; } = null!;

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        public Guid? DiscountCodeId { get; set; }
        public DiscountCode? DiscountCode { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal DiscountAmount { get; set; }

        [NotMapped]
        public decimal Total => Subtotal - DiscountAmount;

        public DateTime? PaidAt { get; set; }

        [Required]
        [MaxLength(30)]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        public string PaymentGateway { get; set; } = null!;

        [MaxLength(50)]
        public string? TransactionId { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = PaymentConst.Status_Pending;

        [MaxLength(1000)]
        public string? PaymentRawData { get; set; }

        [MaxLength(500)]
        public string? InvoiceUrl { get; set; }

        public string? Notes { get; set; }
    }
}
