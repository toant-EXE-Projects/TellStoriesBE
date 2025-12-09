using System.Text.Json.Serialization;

namespace StoryTeller.Data.Enums
{
    // !!!USE ONE!!!
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum eStoryType
    {
        // Type 1
        SinglePanel,
        MultiPanel,

        // Type 2
        Narrative,
        Illustrated
    }
}
