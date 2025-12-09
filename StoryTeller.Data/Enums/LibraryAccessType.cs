using System.Text.Json.Serialization;

namespace StoryTeller.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LibraryAccessType
    {
        Private = 0,
        Unlisted = 1, // Viewable via link
        Public = 2    // Discoverable and shareable
    }
}
