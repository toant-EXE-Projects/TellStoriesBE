namespace StoryTeller.Services.Configurations
{
    public class TokenSettings
    {
        public int Length { get; set; }
        public string Characters { get; set; }
        public int TokenLifespanMinutes { get; set; }
        public string Provider { get; set; }
    }
}
