using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.OptionsModel
{
    public class ChatHistoryFilter
    {
        public Guid SessionId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? RoleFilter { get; set; }
        public bool IncludeErrors { get; set; } = true;
    }
}
