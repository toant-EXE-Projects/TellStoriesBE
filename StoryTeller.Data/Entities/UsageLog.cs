using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class UsageLog : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Type { get; set; } = null!;
        public int UsedSeconds { get; set; }
    }
}
