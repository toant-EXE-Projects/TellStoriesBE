using System.Text.Json.Serialization;

namespace StoryTeller.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WalletTransactionType
    {
        Credit,
        Debit,
        Adjustment
    }
}
