using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System.Security.Claims;

namespace StoryTeller.Services.Interfaces
{
    public interface IUserContextService
    {
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
    }
}
