using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class CommentUpdateRequest
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
    }
}
