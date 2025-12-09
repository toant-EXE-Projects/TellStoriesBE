namespace StoryTeller.Data.Entities
{
    public class CensoredWord : BaseEntity
    {
        public string Word { get; set; } = null!;
        public bool IsWildcard { get; set; }
    }
}
