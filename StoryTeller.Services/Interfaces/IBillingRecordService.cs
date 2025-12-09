namespace StoryTeller.Services.Interfaces
{
    public interface IBillingRecordService
    {
        public Task<int> MarkExpiredRecordsAsync(double minutesFromNow, CancellationToken ct = default);
    }
}
