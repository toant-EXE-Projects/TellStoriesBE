using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class Comment : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid StoryId { get; set; }
        public Story Story { get; set; } = null!;

        public Guid? ReplyTo { get; set; } // Self-referencing FK
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

        [MaxLength(5000)]
        [Required]
        public string Content { get; set; } = null!;
        public bool IsHidden { get; set; }
        public bool IsEdited { get; set; }
        public bool IsFlagged { get; set; }
    }
}
