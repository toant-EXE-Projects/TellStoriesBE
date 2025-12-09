namespace StoryTeller.Services.Models.RequestModel
{
    public class WalletDeltaRequest
    {
        public int Amount { get; set; }
        public string? Reason { get; set; }
    }
}
