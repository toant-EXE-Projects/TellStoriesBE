namespace StoryTeller.Services.Models.RequestModel
{
    public class EmailVerificationCodeRequest
    {
        public string? Subject { get; set; }
        public string? SupportEmail { get; set; }
        public string Recipient { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string BodyPurpose { get; set; } = null!;
        public string Code { get; set; } = null!;

    }
}
