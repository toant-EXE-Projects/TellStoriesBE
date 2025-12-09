using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class StoryPublishRequestDTO
    {
        [Required]
        public Guid Id { get; set; }
        public StoryDTO Story { get; set; } = null!;
        public string? RequestNotes { get; set; }
        public StoryPublishRequestStatus Status { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public UserMinimalDTO? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
