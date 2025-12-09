using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;

namespace StoryTeller.Services.Implementations
{
    public class BillingRecordService : IBillingRecordService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BillingRecordService(IUnitOfWork uow, 
            IDateTimeProvider dateTimeProvider,
            IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<int> MarkExpiredRecordsAsync(double minutesFromNow, CancellationToken ct = default)
        {
            var now = _dateTimeProvider.GetSystemCurrentTime();
            var thresholdTime = now.AddMinutes(minutesFromNow);

            var records = await _uow.BillingRecords.FindAsync(
                    r =>
                        r.Status == PaymentConst.Status_Pending &&
                        !r.PaidAt.HasValue &&
                        r.CreatedDate <= thresholdTime,
                    ct
                );

            if (!records.Any())
                return 0;

            foreach (var record in records)
            {
                record.Status = PaymentConst.Status_Failed;
                record.UpdatedAt = now;
                record.Notes = "Automatically marked as failed by background service.";
            }

            return await _uow.SaveChangesAsync(ct);
        }

    }
}
