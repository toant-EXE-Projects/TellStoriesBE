using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    // JOIN TABLE Story - Tag
    public class StoryTag : BaseEntity
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; } = null!;

        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool IsVisible { get; set; }
    }
}
