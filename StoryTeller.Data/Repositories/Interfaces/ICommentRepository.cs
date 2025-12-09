using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>?> GetDetailedComment();
        Task<List<Comment>?> GetFlaggedComment();
        Task<List<Comment>> GetAllCommentsByStoryAsync(Guid storyId, CancellationToken ct = default);
        Task<int> GetDepthAsync(Guid commentId, CancellationToken ct = default);
        Task<bool> PurgeAllCommentsFromSystem();
        Task<bool> PurgeAllCommentsFromStory(Guid storyId);
        Task<PaginatedResult<Comment>?> GetNewestComments(int page, int pageSize, CancellationToken ct = default);
        Task<Comment?> GetByIdAsync(Guid id);
    }
}
