using StoryTeller.Services.Models.AI.OptionsModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Services.Models.AI.RequestModel
{
    public class PollinationTTSRequest
    {
        /// <summary>
        /// The text prompt to be converted into speech.
        /// </summary>
        [Required(ErrorMessage = "Text is required.")]
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// The voice ID to use for synthesis. Defaults to "alloy".
        /// </summary>  
        [DefaultValue("alloy")]
        public string? VoiceId { get; set; } = "alloy";
        public string? AdditionalInstructions { get; set; } = string.Empty;
        [DefaultValue(true)]
        public bool AutoChunk { get; set; } = true;
        [DefaultValue(300)]
        public int ChunkCharacterLength { get; set; } = 300;
        [DefaultValue(500)]
        public int ChunkDelayms { get; set; } = 500;
    }
}
