namespace StoryTeller.Data.Entities.AI
{
    public class ChatMessage : BaseEntity
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
        public int? TokenCount { get; set; }
        public bool HasError { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public Guid ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;
    }
}
