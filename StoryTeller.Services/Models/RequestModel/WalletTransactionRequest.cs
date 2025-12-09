using StoryTeller.Data.Enums;
using System.ComponentModel;

namespace StoryTeller.Services.Models.RequestModel
{
    public class WalletTransactionRequest
    {
        [DefaultValue(1)]
        public int Page { get; set; } = 1;
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        public string? UserQuery { get; set; }
        public WalletTransactionType? Type { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
