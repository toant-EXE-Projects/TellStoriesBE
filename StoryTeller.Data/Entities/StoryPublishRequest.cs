using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class StoryPublishRequest : BaseEntity
    {
        [Required]
        public Guid StoryId { get; set; }

        public Story Story { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? RequestNotes { get; set; }
        public StoryPublishRequestStatus Status { get; set; } = StoryPublishRequestStatus.Pending;
        public DateTime? ReviewedAt { get; set; }
        public ApplicationUser? ReviewedBy { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? ReviewNotes { get; set; }
    }
}
