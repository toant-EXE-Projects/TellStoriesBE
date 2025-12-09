namespace StoryTeller.Services.Configurations
{
    public class VnPaySettings
    {
        public string TmnCode { get; set; } = null!;
        public string HashSecret { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
        public string ReturnUrl { get; set; } = null!;
    }
}
