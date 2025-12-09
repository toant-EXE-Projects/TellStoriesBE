using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
    }
}
