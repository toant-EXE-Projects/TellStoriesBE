using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class Story : BaseEntity
    {
        [Required]
        [StringLength(100, ErrorMessage = "Story name cannot exceed 100 characters.")]
        public string Title { get; set; }
        [StringLength(50, ErrorMessage = "Author name cannot exceed 50 characters.")]
        public string? Author { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public string? Status { get; set; }
        public bool? IsDraft { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Language { get; set; }
        public double? Duration { get; set; }
        public int ViewCount { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;
        public string? AgeRange { get; set; }
        public string? ReadingLevel { get; set; }
        public int LikeCount { get; set; } = 0;
        public bool IsCommunity { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        [Required]
        public eStoryType StoryType { get; set; } = eStoryType.SinglePanel;
        //=>
        //    Panels != null && Panels.Count > 1
        //        ? eStoryType.MultiPanel
        //        : eStoryType.SinglePanel;
        public bool? IsAIGenerated { get; set; }

        public string? BackgroundMusicUrl { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<StoryPanel> Panels { get; set; }
        public ICollection<FeaturedStories> FeaturedStories { get; set; }
        public ICollection<StoryTag> StoryTags { get; set; } = new List<StoryTag>();
    }
}
