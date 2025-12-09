namespace StoryTeller.Services.Models.RequestModel
{
    public class UserLibraryCreateRequest
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
