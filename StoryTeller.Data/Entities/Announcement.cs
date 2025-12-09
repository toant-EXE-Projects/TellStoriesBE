using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class Announcement : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImgUrl { get; set; }

        public string Type { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
