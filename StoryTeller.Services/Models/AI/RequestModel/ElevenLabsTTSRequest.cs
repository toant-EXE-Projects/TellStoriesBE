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
    public class ElevenLabsTTSRequest
    {
        [Required(ErrorMessage = "Text is required.")]
        public string Text { get; set; } = string.Empty;

        [DefaultValue("7WNWm0yUcEolHsfg5Bhk")]
        public string? VoiceId { get; set; } = "7WNWm0yUcEolHsfg5Bhk";

        [DefaultValue(1)]
        public double? Speed { get; set; } = 1;

        [DefaultValue("vi")]
        public string? LanguageCode { get; set; } = "vi";

        [DefaultValue("eleven_turbo_v2_5")]
        public string? ModelId { get; set; } = "eleven_turbo_v2_5";
        [DefaultValue(true)]
        public bool AutoChunk { get; set; } = true;
        [DefaultValue(300)]
        public int ChunkCharacterLength { get; set; } = 300;
        [DefaultValue(500)]
        public int ChunkDelayms { get; set; } = 500;
        public string? ElevenLabsAPIKey { get; set; }
    }
}
