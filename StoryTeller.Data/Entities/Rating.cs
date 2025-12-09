using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class Rating : BaseEntity
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int Score { get; set; }
        public bool IsHidden { get; set; } = false;
    }
}
