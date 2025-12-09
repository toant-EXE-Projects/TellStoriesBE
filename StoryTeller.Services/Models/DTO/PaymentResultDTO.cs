using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class PaymentResultDTO
    {
        public long PaymentId { get; set; }
        public bool IsSuccess { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public long VnpayTransactionId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
    }
}
