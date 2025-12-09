using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class IssueReportService : IIssueReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public IssueReportService(IUnitOfWork uow,
            IDateTimeProvider dateTimeProvider,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
            _userManager = userManager;
        }
        public async Task<IssueReportDTO> CreateAsync(IssueReportCreateRequest issueReport, ApplicationUser user)
        {
            object? targetObj = null;

            if (!IssueReportConst.IsTargetType(issueReport.TargetType))
                throw new ArgumentException("Issue TargetType must be Comment, Story or Bug.");

            //if (!IssueReportConst.IsIssueTypeComment(issueReport.IssueType))
            //    throw new ArgumentException("Issue TargetType must be Spam, Harassment or Toxic Behavior.");

            issueReport.IssueType = StringUtils.ToUpperFirstAndAfterSpaceChar(issueReport.IssueType);
            issueReport.TargetType = StringUtils.ToUpperFirstAndAfterSpaceChar(issueReport.TargetType);

            if (issueReport.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
            {
                if (issueReport.TargetId == null)
                    throw new ArgumentException("Issue TargetId must not be null when TargetType is Comment or Story");
                
                if ((await _uow.IssueReports.GetCommentReports((Guid)issueReport.TargetId))
                    .FirstOrDefault(ir => ir.UserId == user.Id && ir.CreatedDate > _dateTimeProvider.GetSystemCurrentTime().AddMinutes(-15)) != null)
                {
                    return new IssueReportDTO();
                }

                if ((await _uow.IssueReports.GetCommentReports((Guid)issueReport.TargetId))
                    .FirstOrDefault(ir => ir.UserId == user.Id && ir.CreatedDate > _dateTimeProvider.GetSystemCurrentTime().AddMinutes(-15)) != null)
                {
                    return new IssueReportDTO();
                }

                var comment = await _uow.Comments.GetByIdAsync((Guid)issueReport.TargetId);

                targetObj = _mapper.Map<CommentDTO>(comment);
                if (targetObj == null)
                    throw new NotFoundException($"Comment with id {issueReport.TargetId} is not fount");

                if (!comment!.IsFlagged && (await _uow.IssueReports.NumberOfUserReportedCount(comment.Id)) >= 5)
                {
                    comment.IsFlagged = true;
                    await _uow.Comments.UpdateAsync(comment, user);
                }
            }

            if (issueReport.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
            {
                if (issueReport.TargetId == null)
                    throw new ArgumentException("Issue TargetId must not be null when TargetType is Comment or Story");

                if ((await _uow.IssueReports.GetAllAsync())
                    .FirstOrDefault(ir => ir.TargetId == issueReport.TargetId.ToString() && ir.UserId == user.Id && ir.CreatedDate > _dateTimeProvider.GetSystemCurrentTime().AddMinutes(-15)) != null)
                {
                    return new IssueReportDTO();
                }

                targetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)issueReport.TargetId!));
                if (targetObj == null)
                    throw new NotFoundException($"Story with id {issueReport.TargetId} is not fount");
            }

            var issue = _mapper.Map<IssueReport>(issueReport);
            issue.User = user;
            issue.UserId = user.Id;
            issue.CreatedDate = _dateTimeProvider.GetSystemCurrentTime();
            issue.Status = IssueReportConst.Status.PENDING;

            await _uow.IssueReports.CreateAsync(issue, user);
            await _uow.SaveChangesAsync();

            var result = _mapper.Map<IssueReportDTO>(issue);
            result.TargetObj = targetObj;
            return result;
        }

        public async Task<List<IssueReportDTO>> GetAllAsync()
        {
            var query = await _uow.IssueReports.GetAllDetailAsync();
            var result = _mapper.Map<List<IssueReportDTO>>(query);
            foreach ( var item in result )
            {
                if (item.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<CommentDTO>(await _uow.Comments.GetByIdAsync((Guid)item.TargetId!));
                }

                if (item.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)item.TargetId!));
                }
            }
            return result;
        }

        public async Task<PaginatedResult<IssueReportDTO>> GetAllAsPaginatedResultAsync(int page = 1, int pageSize = 10)
        {
            var query = await _uow.IssueReports.GetAllDetailAsPaginatedResultAsync(page, pageSize);
            var result = _mapper.Map<PaginatedResult<IssueReportDTO>>(query);
            foreach (var item in result.Items)
            {
                if (item.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<CommentDTO>(await _uow.Comments.GetByIdAsync((Guid)item.TargetId!));
                }

                if (item.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)item.TargetId!));
                }
            }
            return result;
        }

        public async Task<IssueReportDTO> GetByIdAsync(Guid id)
        {
            var query = await _uow.IssueReports.GetByIdAsync(id);
            if (query == null)
                throw new NotFoundException($"IssueReport with ID {id} not found.");

            var result = _mapper.Map<IssueReportDTO>(query);
            if (result.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
            {
                result.TargetObj = _mapper.Map<CommentDTO>(await _uow.Comments.GetByIdAsync((Guid)result.TargetId!));
            }

            if (result.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
            {
                result.TargetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)result.TargetId!));
            }

            return result;
        }

        public async Task<List<IssueReportDTO>> GetByUserIdAsync(string id)
        {
            var query = await _uow.IssueReports.GetByUserId(id);
            var result = _mapper.Map<List<IssueReportDTO>>(query);
            foreach (var item in result)
            {
                if (item.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<CommentDTO>(await _uow.Comments.GetByIdAsync((Guid)item.TargetId!));
                }

                if (item.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
                {
                    item.TargetObj = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)item.TargetId!));
                }
            }
            return result;
        }

        public async Task<List<IssueReportDTO>> GetReportedIssueByUserId(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found"); 
            
            var query = await _uow.IssueReports.GetAllDetailAsync();
            var issues = _mapper.Map<List<IssueReportDTO>>(query);
            var result = _mapper.Map<List<IssueReportDTO>>(query);

            foreach (var item in issues)
            {
                if (item.TargetType.Equals(IssueReportConst.TargetType.BUG, StringComparison.OrdinalIgnoreCase))
                {
                    result.Remove(result.First(i => i.Id == item.Id));
                    continue;
                }
                if (item.TargetType.Equals(IssueReportConst.TargetType.COMMENT, StringComparison.OrdinalIgnoreCase))
                {
                    var comment = await _uow.Comments.GetByIdAsync((Guid)item.TargetId!);
                    if (comment == null || comment.UserId != user.Id)
                    {
                        result.Remove(result.First(i => i.Id == item.Id));
                        continue;
                    }
                    result.First(i => i.Id == item.Id).TargetObj = _mapper.Map<CommentDTO>(comment);
                }

                if (item.TargetType.Equals(IssueReportConst.TargetType.STORY, StringComparison.OrdinalIgnoreCase))
                {
                    var target = _mapper.Map<StoryDTO>(await _uow.Stories.GetStoryWithMinimumDetails((Guid)item.TargetId!));
                    if (target == null || target.CreatedBy == null || (target.CreatedBy != null && target.CreatedBy.Id != user.Id))
                    {
                        result.Remove(result.First(i => i.Id == item.Id));
                        continue;
                    }
                    result.First(i => i.Id == item.Id).TargetObj = target;
                }
            }
            return result;
        }

        public async Task<IssueReportDTO> ResolveAsync(Guid id, ApplicationUser user)
        {
            var issue = await _uow.IssueReports.GetByIdAsync(id);
            if (issue == null)
            {
                throw new NotFoundException("Issue not found!");
            }

            issue.Status = IssueReportConst.Status.RESOLVED;

            await _uow.IssueReports.UpdateAsync(issue, user);
            await _uow.SaveChangesAsync();

            var result = _mapper.Map<IssueReportDTO>(issue);
            return result;
        }

        public async Task<IssueReportDTO> DeleteAsync(Guid id, ApplicationUser user)
        {
            var issue = await _uow.IssueReports.GetByIdAsync(id);
            if (issue == null)
            {
                throw new NotFoundException("Issue not found!");
            }

            await _uow.IssueReports.SoftRemove(issue, user);
            await _uow.SaveChangesAsync();

            var result = _mapper.Map<IssueReportDTO>(issue);
            return result;
        }
    }
}
