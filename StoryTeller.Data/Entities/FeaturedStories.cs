using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class FeaturedStories : BaseEntity
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public int Priority { get; set; }
    }
}
