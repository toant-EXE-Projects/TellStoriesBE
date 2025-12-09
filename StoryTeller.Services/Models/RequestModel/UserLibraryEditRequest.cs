using StoryTeller.Data.Enums;

namespace StoryTeller.Services.Models.RequestModel
{
    public class UserLibraryEditRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public LibraryAccessType? AccessType { get; set; }
    }
}
