using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public required DateTime DOB { get; set; }
    }
}
