using System.Text.Json.Serialization;

namespace StoryTeller.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SubscriptionPurchaseMethod
    {
        MoneyOnly,
        PointsOnly,
        MoneyOrPoints
    }
}
