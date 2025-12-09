using AutoMapper;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Data.Utils;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;

namespace StoryTeller.Services.Implementations
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ActivityLogService(IUnitOfWork uow, 
            IDateTimeProvider dateTimeProvider,
            IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }
        public async Task<ActivityLogDTO> CreateAsync(ActivityLogCreateRequest activityLog, ApplicationUser user)
        {
            object? targetObj = null;

            if (!ActivityLogConst.IsTargetType(activityLog.TargetType))
                throw new ArgumentException("ActivityLog TargetType must be Bình luận hoặc Truyện.");

            if (!ActivityLogConst.IsAction(activityLog.Action))
                throw new ArgumentException("ActivityLog Action must be Hệ thống, Xuất bản truyện, Bình luận, Đánh giá hoặc Xem truyện.");

            activityLog.TargetType = StringUtils.ToUpperFirstAndAfterSpaceChar(activityLog.TargetType);
            activityLog.Action = StringUtils.ToUpperFirstAndAfterSpaceChar(activityLog.Action);

            if (activityLog.TargetType.Equals(ActivityLogConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
            {
                if (activityLog.TargetId == null)
                    throw new ArgumentException("ActivityLog TargetId must not be null when TargetType is Bình luận hoặc Truyện");

                targetObj = _mapper.Map<CommentDTO>(await _uow.Comments.GetByIdAsync((Guid)activityLog.TargetId));
                if (targetObj == null)
                    throw new NotFoundException($"Comment with id {activityLog.TargetId} is not fount");
            }

            if (activityLog.TargetType.Equals(ActivityLogConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
            {
                if (activityLog.TargetId == null)
                    throw new ArgumentException("ActivityLog TargetId must not be null when TargetType is Bình luận hoặc Truyện");

                targetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)activityLog.TargetId!));
                if (targetObj == null)
                    throw new NotFoundException($"Story with id {activityLog.TargetId} is not fount");
            }

            var log = _mapper.Map<ActivityLog>(activityLog);
            log.User = user;
            log.UserId = user.Id;
            log.Timestamp = _dateTimeProvider.GetSystemCurrentTime();

            await _uow.ActivityLogs.CreateAsync(log, user);
            await _uow.SaveChangesAsync();

            return _mapper.Map<ActivityLogDTO>(log);
        }

        public async Task<List<ActivityLogDTO>> GetAllAsync()
        {
            var result = await _uow.ActivityLogs.GetAllDetailAsync();
            return _mapper.Map<List<ActivityLogDTO>>(result);
        }

        public async Task<ActivityLogDTO> GetByIdAsync(Guid id)
        {
            var result = await _uow.ActivityLogs.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"ActivityLog with ID {id} not found.");

            return _mapper.Map<ActivityLogDTO>(result);
        }

        public async Task<List<ActivityLogDTO>> GetByUserIdAsync(string id, DateTime? getByDateFrom = null, DateTime? getByDateTo = null, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var dateFrom = getByDateFrom ?? DateTime.MinValue;
            var dateTo = getByDateTo ?? DateTime.MaxValue;

            if (dateFrom.Date > dateTo.Date)
                throw new ArgumentException("DateFrom must be before DateTo ");

            var result = await _uow.ActivityLogs.GetByUserId(id, dateFrom.Date, dateTo.Date);

            return _mapper.Map<List<ActivityLogDTO>>(result);
        }

        public async Task<bool> ClearLogAsync()
        {
            var result = await _uow.ActivityLogs.ClearLogAsync();
            return result >= 0;
        }

        public async Task<List<ActivityLogDTO>> GetAllByDateAsync(DateTime? date)
        {
            var result = await _uow.ActivityLogs.GetAllDetailAsync();
            return _mapper.Map<List<ActivityLogDTO>>(result);
        }

        public class ActivityLogCreateObj
        {
            public string TargetType { get; set; } = null!;
            public string Action { get; set; } = null!;
            public DateTime Timestamp { get; set; }

            public string? Details { get; set; }
            public string? Category { get; set; }
            public Guid? TargetId { get; set; }
            public string? Reason { get; set; }
            public string? ActorRole { get; set; }
            public string? DeviceInfo { get; set; }
        }
    }
}
