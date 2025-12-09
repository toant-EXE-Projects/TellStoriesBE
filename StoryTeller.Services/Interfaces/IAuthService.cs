using Microsoft.AspNetCore.Identity;
using StoryTeller.Data.Entities;
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
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest request);
        Task<string?> RegisterAsync(RegisterRequest request);
        Task<APIResponse<object>> ForgotPasswordAsync(string email);
        Task<APIResponse<object>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<APIResponse<object>> ConfirmEmailAsync(string email, string token);
        Task<APIResponse<object>> SendEmailConfirmationTokenAsync(string email);
        Task<APIResponse<object>> VerifyAuthToken(VerifyTokenRequest request);
    }
}
