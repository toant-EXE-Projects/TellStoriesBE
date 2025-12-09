using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryPublishReviewRequest
    {
        [Required]
        public Guid Id { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? ReviewNotes { get; set; }
    }
}
