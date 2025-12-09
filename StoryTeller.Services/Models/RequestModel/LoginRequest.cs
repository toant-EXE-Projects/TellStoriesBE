using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
