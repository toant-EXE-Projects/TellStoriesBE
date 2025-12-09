using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDTO> GetByIdAsync(Guid id);
        Task<List<CommentDTO>> GetAllAsync();
        Task<PaginatedResult<CommentDTO>> GetNewestAsync(int page, int pageSize, CancellationToken ct = default);
        Task<List<CommentDTO>> GetFlaggedComment();
        Task<CommentDTO> CreateAsync(CommentCreateRequest request, ApplicationUser user, CancellationToken ct = default);
        Task<CommentDTO> EditAsync(CommentUpdateRequest request, ApplicationUser user, CancellationToken ct = default);
        Task<CommentDTO> EditMetaAsync(Guid id, CommentMeta meta, ApplicationUser user, CancellationToken ct = default);
        Task<bool> DeleteCommentAsync(Guid commentId, ApplicationUser user, CancellationToken ct = default);
        Task<bool> HardDeleteCommentAsync(Guid commentId, CancellationToken ct = default);
        Task<PaginatedResult<CommentThreadDTO>> GetCommentThreadAsync(Guid storyId, int page, int pageSize, CancellationToken ct = default);
        public Task<CommentThreadDTO> GetThreadFromCommentAsync(Guid commentId, CancellationToken ct = default);
        Task<bool> PurgeAllCommentsFromSystemAsync(CancellationToken ct = default);
        Task<bool> PurgeAllCommentsFromStoryAsync(Guid storyId, CancellationToken ct = default);
        Task<List<HeatMapReportedCommentResponse>> HeatMapReportedComment(Guid storyId);
    }
}
