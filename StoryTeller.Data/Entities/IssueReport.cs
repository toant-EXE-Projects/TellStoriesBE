using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class IssueReport : BaseEntity
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string TargetType { get; set; } = null!; // Story, Comment, Bug, Other
        public string? IssueType { get; set; }
        public string? TargetId { get; set; }
        public string? Attachment { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
}
