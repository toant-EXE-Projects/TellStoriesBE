using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class CommentThreadDTO
    {
        public Guid Id { get; set; }
        public Guid StoryId { get; set; }
        public UserMinimalDTO User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public List<CommentThreadDTO> Replies { get; set; } = new();
        public DateTime CreatedDate { get; set; }
    }
}
