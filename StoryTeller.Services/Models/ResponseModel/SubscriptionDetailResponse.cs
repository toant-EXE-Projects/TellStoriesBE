using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class SubscriptionDetailResponse
    {
        public string? UserId { get; set; }
        public string? User { get; set; }
        public string? Plan { get; set; }
        public int Price { get; set; } = 0;
        public int? Duration { get; set; }
        public DateTime? SubscribedOn { get; set; }
        public DateTime? OriginalEndDate { get; set; }
        public DateTime? ExpiriesOn { get; set; }
        public int DayRemaining { get; set; } = 0;
    }
}
