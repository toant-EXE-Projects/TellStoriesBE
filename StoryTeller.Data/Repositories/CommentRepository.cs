using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Models;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly StoryTellerContext _context;
        private readonly ILoggerService _logger;

        public CommentRepository(StoryTellerContext context, IDateTimeProvider dateTimeProvider, ILoggerService logger)
        : base(context, dateTimeProvider)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Comment>?> GetDetailedComment()
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => !c.IsHidden && !c.IsFlagged)
                .ToListAsync();
        }

        public async Task<PaginatedResult<Comment>?> GetNewestComments(int page, int pageSize, CancellationToken ct = default)
        {
            return await _dbSet
                .OrderByDescending(c => c.CreatedDate)
                .Where(c => 
                    !c.IsHidden && 
                    !c.IsFlagged
                )
                .ToPaginatedResultAsync(page, pageSize, ct);
        }

        public async Task<List<Comment>> GetAllCommentsByStoryAsync(Guid storyId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => 
                    c.StoryId == storyId && 
                    !c.IsHidden && 
                    !c.IsFlagged
                 )
                .OrderBy(c => c.CreatedDate)
                .ToListAsync(ct);
        }

        public async Task<List<Comment>?> GetFlaggedComment()
        {
            return await _dbSet
                .Where(c => c.IsFlagged)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<bool> PurgeAllCommentsFromSystem()
        {
            try
            {
                var allComments = await _dbSet.ToListAsync();
                if (allComments.Count == 0)
                    return false;

                _dbSet.RemoveRange(allComments);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to purge all comments from system.", ex);
                return false;
            }
        }

        public async Task<bool> PurgeAllCommentsFromStory(Guid storyId)
        {
            try
            {
                var storyComments = await _dbSet
                    .Where(c => c.StoryId == storyId)
                    .ToListAsync();

                if (storyComments.Count == 0)
                    return false;

                _dbSet.RemoveRange(storyComments);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to purge comments for story {storyId}.", ex);
                return false;
            }
        }

        public async Task<int> GetDepthAsync(Guid commentId, CancellationToken ct = default)
        {
            int depth = 0;
            const int maxDepth = SystemConst.Comment_MaxDepth;
            var visited = new HashSet<Guid>();

            Comment? current = await _dbSet
                .Where(c => c.Id == commentId)
                .Select(c => new Comment { Id = c.Id, ReplyTo = c.ReplyTo })
                .FirstOrDefaultAsync(ct);

            if (current == null)
                return -1;

            while (current?.ReplyTo != null)
            {
                if (!visited.Add(current.Id))
                    throw new InvalidOperationException("Cycle detected in comment replies.");

                depth++;

                if (depth >= maxDepth)
                    break;

                current = await _dbSet
                    .Where(c => c.Id == current.ReplyTo)
                    .Select(c => new Comment { Id = c.Id, ReplyTo = c.ReplyTo })
                    .FirstOrDefaultAsync(ct);
            }

            return depth;
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => !c.IsHidden && !c.IsFlagged && c.Id == id);
        }
    }
}