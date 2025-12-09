using StoryTeller.Data.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.RequestModel
{
    public class SystemConfigUpdateRequest
    {
        [Required]
        [DefaultValue("0")]
        public string Value { get; set; } = null!;
    }
}
