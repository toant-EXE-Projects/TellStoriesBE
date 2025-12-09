using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.ResponseModel
{
    public class StoryGenerationResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }

        [JsonPropertyName("ageRange")]
        public string AgeRange { get; set; }

        [JsonPropertyName("readingLevel")]
        public string ReadingLevel { get; set; }

        [JsonPropertyName("isAIGenerated")]
        public bool IsAIGenerated { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();
    }
}
