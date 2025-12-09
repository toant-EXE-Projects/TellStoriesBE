using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class CommentDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public UserMinimalDTO User { get; set; } = null!;

        public Guid StoryId { get; set; }
        public string Content { get; set; } = null!;

        public CommentDTO? ParentComment { get; set; }
        public Guid? ReplyTo { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsHidden { get; set; }
        public bool IsEdited { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFlagged { get; set; }
    }
}
