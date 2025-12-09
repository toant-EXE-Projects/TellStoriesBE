using Microsoft.AspNetCore.Identity;
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
    public interface IUserService
    {
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(string id);
        Task<UserDTO?> GetByEmailAsync(string email);
        Task<UserDTO?> CreateAsync(UserCreateRequest request);
        Task<bool> UpdateAsync(string id, UserUpdateRequest request, ApplicationUser currentUser);
        Task<bool> DeleteAsync(string id, ApplicationUser currentUser);
        Task<bool> DeleteOwnAccountAsync(string userId, string currentPassword, ApplicationUser currentUser);
        Task<int> UpdateLastLoginAsync(string id, CancellationToken ct = default);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<List<UserDTO>> SearchAsync(string searchValue);
        Task<bool> ChangeRoleAsync(string id, string role);
        Task<DashboardResponse> Dashboard(int statisticPeriod);
        Task<PaginatedResult<UserDTO>> QueryUser(UserQueryRequest request);
        Task<SubscriptionDetailResponse> GetSubscriptionDetail(string userId);
        Task<BillingHistoryResponse> BillingHistory(string userId, bool? ascOrder, int page = 1, int pageSize = 10);
        Task<PaginatedResult<BillingRecordDTO>> BillingHistoryQuery(string? query, bool? ascOrder, int page = 1, int pageSize = 10);
        Task<List<BillingRecordDTO>> RevenueDetail(int period);
    }
}
