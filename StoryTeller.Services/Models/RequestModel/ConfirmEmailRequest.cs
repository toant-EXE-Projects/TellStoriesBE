using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class ConfirmEmailRequest
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
