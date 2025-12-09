using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class UserCreateRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public string UserType { get; set; }
        public string? Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime? DOB { get; set; }
    }
}
