using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.RequestModel
{
    public class PolinationImageGenerationRequest
    {
        [Required]
        public string Prompt { get; set; } = string.Empty;
        [DefaultValue(512)]
        public int? Width { get; set; } = 512;
        [DefaultValue(512)]
        public int? Height { get; set; } = 512;
        [DefaultValue("flux")]
        public string? ModelId { get; set; } = "flux"; // flux, kontext, turbo, gptimage
        public int? Seed { get; set; }
    }
}
