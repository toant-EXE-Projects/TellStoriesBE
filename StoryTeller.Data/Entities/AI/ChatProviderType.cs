using System.Text.Json.Serialization;

namespace StoryTeller.Data.Entities.AI
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChatProviderType
    {
        Gemini,
        ChatGPT
    }
}
