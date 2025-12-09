using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class StoryDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public double? Duration { get; set; }
        public int ViewCount { get; set; } = 0;
        public string? AgeRange { get; set; }
        public string? ReadingLevel { get; set; }
        public eStoryType StoryType { get; set; }
        public bool? IsAIGenerated { get; set; }
        public string? BackgroundMusicUrl { get; set; }
        public bool? IsFeatured { get; set; } = false;
        public bool? IsCommunity { get; set; } = false;
        public bool? IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserMinimalDTO? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserMinimalDTO? UpdatedBy { get; set; }
        public List<StoryPanelCreateRequest> Panels { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<TagDTO> Tags { get; set; } = new List<TagDTO>();
        public List<string> TagNames { get; set; } = new List<string>();
    }
}
