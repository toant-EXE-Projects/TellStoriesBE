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
    public class ViettelTTSRequest
    {

        [Required(ErrorMessage = "Text is required.")]
        public string Text { get; set; } = string.Empty;

        [DefaultValue("hcm-diemmy")]
        public string? VoiceID { get; set; } = "hcm-diemmy";

        [Range(0.5, 2.0)]
        [DefaultValue(1)]
        public float? Speed { get; set; } = 1;

        [DefaultValue(false)]
        public bool? WithoutFilter { get; set; } = false;

        [DefaultValue(3)]
        [JsonIgnore]
        public int? ReturnOption { get; set; }
        [DefaultValue(true)]
        public bool AutoChunk { get; set; } = true;
        [DefaultValue(300)]
        public int ChunkCharacterLength { get; set; } = 300;
        [DefaultValue(500)]
        public int ChunkDelayms { get; set; } = 500;
        [DefaultValue("")]
        public string Token { get; set; } = string.Empty;
    }
}
