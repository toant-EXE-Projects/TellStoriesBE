using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Description { get; set; }
        public bool? IsDraft { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public double? Duration { get; set; }
        public string? AgeRange { get; set; }
        public string? ReadingLevel { get; set; }
        public eStoryType StoryType { get; set; }
        public bool? IsAIGenerated { get; set; }
        public string? BackgroundMusicUrl { get; set; }
        public List<StoryPanelCreateRequest>? Panels { get; set; }
        public AddTagsRequest? Tags { get; set; }
    }
}
