using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.OptionsModel
{
    public class ChatGenerationOptions
    {
        public double? Temperature { get; set; } = 0.6f;
        public double? TopP { get; set; } = 0.8f;
        public double? TopK { get; set; } = 20;
        public int? MaxOutputTokens { get; set; } = 8192;
        //public int? CandidateCount { get; set; }
        public List<string>? StopSequences { get; set; }
        public int? Seed { get; set; }
        public string? AdditionalSystemInstruction { get; set; }
        [JsonIgnore]
        public string? SystemInstruction { get; set; }
        [JsonIgnore]
        public object? ProviderConfig { get; set; }
    }
}
