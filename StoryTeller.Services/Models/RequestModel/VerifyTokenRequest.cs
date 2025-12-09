using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StoryTeller.Services.Models.RequestModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TokenPurpose
    {
        EmailConfirmation,
        ResetPassword,
        TwoFactorAuth
    }
    public class VerifyTokenRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public TokenPurpose Purpose { get; set; }
    }
}
