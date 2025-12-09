using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class IssueReportDTO
    {
        public Guid Id { get; set; }
        public UserMinimalDTO? User { get; set; }

        public string IssueType { get; set; } = null!;
        public string TargetType { get; set; } = null!;
        public Guid? TargetId { get; set; }
        public object? TargetObj { get; set; }
        public string? Attachment { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
