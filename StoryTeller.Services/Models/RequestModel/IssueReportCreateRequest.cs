using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class IssueReportCreateRequest
    {
        [Required]
        public string TargetType { get; set; }
        [Required]
        public string Description { get; set; }
        public string? IssueType { get; set; } = string.Empty;
        public Guid? TargetId { get; set; }
        public string? Attachment { get; set; }
    }
}
