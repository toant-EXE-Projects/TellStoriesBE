namespace StoryTeller.Services.Models.RequestModel
{
    public class UserLibraryItemCreateRequest
    {
        public Guid StoryId { get; set; }
        public Guid UserCollectionId { get; set; }
    }
}
