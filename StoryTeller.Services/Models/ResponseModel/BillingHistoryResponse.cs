using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class BillingHistoryResponse
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int RemainSubscriptionDays { get; set; } = 0;
        public int TotalMoneySpent { get; set; } = 0;

        public string? SubscriptionName { get; set; }
        public int SubscriptionPrice { get; set; } = 0;
        public int SubscriptionDurationDays { get; set; } = 0;
        public DateTime? SubscriptionEndDate { get; set; }

        public PaginatedResult<BillingRecordDTO> BillingHistory { get; set; } = new PaginatedResult<BillingRecordDTO>();
    }
}
