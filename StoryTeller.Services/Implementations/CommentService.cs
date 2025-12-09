using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using StoryTeller.Data;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Services.Errors;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using StoryTeller.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly ICensorService _censorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentService(IUnitOfWork uow, 
            IMapper mapper, 
            ILoggerService logger, 
            ICensorService censorService, 
            UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
            _censorService = censorService;
            _userManager = userManager;
        }

        public async Task<CommentDTO> CreateAsync(CommentCreateRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var story = await _uow.Stories.GetByIdAsync(request.StoryId, ct);
            if (story == null)
                throw new NotFoundException("Story not found");

            if (!story.IsPublished || !story.IsCommunity)
                throw new InvalidOperationException("You can only comment on published community stories.");

            request.Content = await _censorService.FilterContentAsync(request.Content);
            var comment = _mapper.Map<Comment>(request);
            comment.User = user;
            comment.UserId = user.Id;

            if (request.ReplyTo.HasValue)
            {
                var replyToComment = await _uow.Comments.GetByIdAsync((Guid)request.ReplyTo, ct);
                if (replyToComment == null) 
                    throw new NotFoundException("ReplyTo comment not found.");
                if (replyToComment.IsDeleted)
                    throw new InvalidOperationException("You can't reply to deleted comments");
                if (replyToComment.IsFlagged)
                    throw new InvalidOperationException("You can't reply to flagged comments");

                var depth = await _uow.Comments.GetDepthAsync(request.ReplyTo.Value);
                if (depth >= SystemConst.Comment_MaxDepth)
                    throw new InvalidOperationException("Maximum reply depth exceeded.");
            }

            try
            {
                await _uow.Comments.CreateAsync(comment, user);
                await _uow.SaveChangesAsync();

                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating comment.", ex);
                throw;
            }
        }

        public async Task<CommentDTO> EditAsync(CommentUpdateRequest request, ApplicationUser user, CancellationToken ct = default)
        {
            var comment = await _uow.Comments.GetByIdAsync(request.Id, ct);
            if (comment == null) { throw new NotFoundException("Comment not found."); }
            if (comment.UserId != user.Id) { throw new UnauthorizedAccessException("Only user who write the comment can edit it."); }
            if (comment.IsDeleted) throw new UnauthorizedAccessException("You cannot edit deleted comments");
            if (comment.IsFlagged) throw new UnauthorizedAccessException("Your comment is flagged for review.");

            request.Content = await _censorService.FilterContentAsync(request.Content);
            comment.Content = request.Content;
            try
            {
                comment.IsEdited = true;
                await _uow.Comments.UpdateAsync(comment, user, ct);
                await _uow.SaveChangesAsync(ct);

                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error editing comment.", ex);
                throw;
            }
        }

        public async Task<CommentDTO> EditMetaAsync(Guid id, CommentMeta meta, ApplicationUser user, CancellationToken ct = default)
        {
            var comment = await _uow.Comments.GetByIdAsync(id, ct);
            if (comment == null) throw new NotFoundException("Comment not found!");

            try
            {
                _mapper.Map(meta, comment);
                await _uow.Comments.UpdateAsync(comment, user, ct);
                await _uow.SaveChangesAsync(ct);

                return _mapper.Map<CommentDTO>(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error editing comment metadata.", ex);
                throw;
            }
        }

        public async Task<bool> PurgeAllCommentsFromSystemAsync(CancellationToken ct = default)
        {
            try
            {
                var deleted = await _uow.Comments.PurgeAllCommentsFromSystem();
                if (!deleted)
                    return false;

                await _uow.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error purging all comments from system", ex);
                return false;
            }
        }

        public async Task<bool> PurgeAllCommentsFromStoryAsync(Guid storyId, CancellationToken ct = default)
        {
            try
            {
                var deleted = await _uow.Comments.PurgeAllCommentsFromStory(storyId);
                if (!deleted)
                    return false;

                await _uow.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error purging comments from story {storyId}", ex);
                return false;
            }
        }

        public async Task<List<CommentDTO>> GetAllAsync()
        {
            var result = await _uow.Comments.GetDetailedComment();
            return _mapper.Map<List<CommentDTO>>(result);
        }

        public async Task<PaginatedResult<CommentDTO>> GetNewestAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var result = await _uow.Comments.GetNewestComments(page, pageSize, ct);
            return _mapper.Map<PaginatedResult<CommentDTO>>(result);
        }

        public async Task<List<CommentDTO>> GetFlaggedComment()
        {
            var result = await _uow.Comments.GetFlaggedComment();
            return _mapper.Map<List<CommentDTO>>(result);
        }

        public async Task<CommentDTO> GetByIdAsync(Guid id)
        {
            var result = await _uow.Comments.GetByIdAsync(id);
            if (result == null)
                throw new NotFoundException($"ActivityLog with ID {id} not found.");

            return _mapper.Map<CommentDTO>(result);
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, ApplicationUser user, CancellationToken ct = default)
        {
            var cmt = await _uow.Comments.GetByIdAsync(commentId, ct);
            if (cmt == null)
                throw new NotFoundException("Comment not found");

            if (!await user.IsStaffAsync(_userManager))
            {
                if (cmt.UserId != user.Id)
                    throw new UnauthorizedAccessException("You are not allowed to delete this comment.");
            }

            cmt.Content = StringConstants.DeletedCommentContent;

            await _uow.Comments.SoftRemove(cmt, user, ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> HardDeleteCommentAsync(Guid commentId, CancellationToken ct = default)
        {
            var cmt = await _uow.Comments.GetByIdAsync(commentId, ct);
            if (cmt == null)
                throw new NotFoundException("Comment not found");

            _uow.Comments.Remove(cmt);
            await _uow.SaveChangesAsync(ct);
            return true;
        }


        private CommentThreadDTO BuildCommentThread(Comment root, ILookup<Guid?, Comment> lookup)
        {
            return new CommentThreadDTO
            {
                Id = root.Id,
                StoryId = root.StoryId,
                User = _mapper.Map<UserMinimalDTO>(root.User),
                Content = root.Content,
                CreatedDate = root.CreatedDate,
                Replies = BuildReplies(root.Id, lookup)
            };
        }

        private List<CommentThreadDTO> BuildReplies(Guid? parentId, ILookup<Guid?, Comment> lookup)
        {
            return lookup[parentId]
                .OrderBy(c => c.CreatedDate)
                .Select(reply => BuildCommentThread(reply, lookup))
                .ToList();
        }

        public async Task<PaginatedResult<CommentThreadDTO>> GetCommentThreadAsync(Guid storyId, int page, int pageSize, CancellationToken ct = default)
        {
            var story = await _uow.Stories.GetByIdAsync(storyId, ct);
            if (story == null)
                throw new NotFoundException("Story not found");

            // get all non-hidden, non-deleted comments
            var allComments = await _uow.Comments.GetAllCommentsByStoryAsync(storyId, ct);

            // filter top-level comments for pagination
            var topLevel = allComments
                .Where(c => c.ReplyTo == null)
                .OrderByDescending(c => c.CreatedDate);

            var totalItems = topLevel.Count();
            var pagedTopLevel = topLevel
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // build full threaded result, but only include replies of the paginated top-level comments
            var lookup = allComments.ToLookup(c => c.ReplyTo);

            var threads = pagedTopLevel
                    .Select(comment => BuildCommentThread(comment, lookup))
                    .ToList();

            return new PaginatedResult<CommentThreadDTO>
            {
                CurrentPage = page,
                PageCount = pageSize,
                TotalItems = totalItems,
                Items = threads
            };
        }
        
        public async Task<CommentThreadDTO> GetThreadFromCommentAsync(Guid commentId, CancellationToken ct = default)
        {
            var comment = await _uow.Comments.GetByIdAsync(commentId, ct);
            if (comment == null)
                throw new NotFoundException("Comment not found.");

            var story = await _uow.Stories.GetByIdAsync(comment.StoryId, ct);
            if (story == null)
                throw new NotFoundException("Story not found.");

            // walk up to the root comment
            var current = comment;
            while (current.ReplyTo != null)
            {
                var parent = await _uow.Comments.GetByIdAsync(current.ReplyTo.Value, ct);
                if (parent == null)
                    break;
                current = parent;
            }

            var rootComment = current;

            var allComments = await _uow.Comments.GetAllCommentsByStoryAsync(story.Id, ct);
            var lookup = allComments.ToLookup(c => c.ReplyTo);

            return BuildCommentThread(rootComment, lookup);
        }
        public async Task<List<HeatMapReportedCommentResponse>> HeatMapReportedComment(Guid storyId)
        {
            List<HeatMapReportedCommentResponse> result = [];
            var comments = await _uow.Comments.GetAllCommentsByStoryAsync(storyId);
            foreach (var comment in _mapper.Map<List<CommentDTO>>(comments))
            {
                comment.ParentComment = null;
                var heatmapItem = result.FirstOrDefault(r => r.User.Id == comment.User.Id);
                if (heatmapItem is null)
                {
                    result.Add(new HeatMapReportedCommentResponse
                    {
                        User = comment.User,
                        ReportedComments = new List<ReportedComment>([
                            new ReportedComment
                            {
                                Comment = comment,
                                ReportCount = (await _uow.IssueReports.GetCommentReports(comment.Id)).Count
                            }])
                    });
                }
                else
                {
                    heatmapItem.ReportedComments.Add(new ReportedComment
                    {
                        Comment = comment,
                        ReportCount = (await _uow.IssueReports.GetCommentReports(comment.Id)).Count
                    });
                }
            }
            return result;
        }
    }
}
