using System.Text.Json.Serialization;

namespace StoryTeller.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StoryPublishRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3,
    }
}
