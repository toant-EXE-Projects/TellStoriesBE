namespace StoryTeller.Data.Entities.AI
{
    public class ChatSession : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        public string? Title { get; set; }
        public List<ChatMessage> Messages { get; set; } = new();
        public string? ProviderUsed { get; set; }
        public string? Language { get; set; }
        public bool IsArchived { get; set; } = false;
    }
}
