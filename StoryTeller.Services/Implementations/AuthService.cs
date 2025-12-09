using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Configurations;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static StoryTeller.Data.Constants.ActivityLogConst;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StoryTeller.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly TokenSettings _tokenOptions;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggerService _logger;

        public AuthService(
            IUnitOfWork uow,
            IConfiguration config,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IUserService userService,
            IDateTimeProvider dateTimeProvider,
            ILoggerService loggerService
        )
        {
            _uow = uow;
            _config = config;
            _userManager = userManager;
            _emailService = emailService;
            _userService = userService;
            _dateTimeProvider = dateTimeProvider;
            _tokenOptions = _config.GetSection("TokenOptions").Get<TokenSettings>() ?? new TokenSettings();
            _logger = loggerService;
        }

        public async Task<string?> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Name, user.DisplayName ?? user.UserName ?? ""),
                new Claim("AvatarUrl", user.AvatarUrl ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]!));

            int JWTLifeTime = int.TryParse(_config["JwtSettings:LifeTimeMinutes"], out var minutes)
                                ? minutes : 60;
            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: _dateTimeProvider.GetSystemCurrentTime().AddMinutes(JWTLifeTime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            await _uow.ActivityLogs.CreateAsync(new ActivityLog
            {
                UserId = user.Id,
                TargetType = ActivityLogConst.TargetType.USER,
                Action = ActivityLogConst.Action.SYSTEM,
                Timestamp = _dateTimeProvider.GetSystemCurrentTime(),
                Details = $"{_dateTimeProvider.GetSystemCurrentTime().ToString("dd-MM-yyyy")} - {user.DisplayName} (hệ thống): Người dùng đã đăng nhập thành công",
                Category = "Hệ thống",
                TargetId = new Guid(user.Id),
                Reason = "",
                ActorRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
                DeviceInfo = ""
            }, user);

            await _userService.UpdateLastLoginAsync(user.Id);

            await _uow.SaveChangesAsync();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string?> RegisterAsync(RegisterRequest request)
        {
            var matchEmail = await _userManager.FindByEmailAsync(request.Email);
            if (matchEmail != null)
                return null;

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                DisplayName = request.DisplayName ?? request.Email,
                UserType = Roles.User,
                PhoneNumber = request.PhoneNumber,
                AvatarUrl = request.AvatarUrl,
                CreatedDate = _dateTimeProvider.GetSystemCurrentTime(),
                UpdatedDate = _dateTimeProvider.GetSystemCurrentTime(),
                DOB = request.DOB,
                Status = UserStatusConstants.Active
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return null;

            await _userManager.AddToRoleAsync(user, Roles.User);

            return user.Id; // return userId for further initialization
        }

        public async Task<APIResponse<object>> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                // Send a success response but don't confirm that there is or isn't a user on the server
                return APIResponse<object>.SuccessResponse("A reset token has been sent to your email");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (user.Email == null)
                return APIResponse<object>.ErrorResponse("User email is null");


            try
            {
                var passwordResetRequest = new EmailVerificationCodeRequest
                {
                    Recipient = user.Email,
                    DisplayName = user.DisplayName ?? user.Email,
                    BodyPurpose = EmailTemplates.PasswordResetBodyPurpose,
                    Code = token
                };
                await _emailService.SendVerificationCodeEmailAsync(passwordResetRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't send Password Reset verification code. Exception: {ex.Message}");
            }

            return APIResponse<object>.SuccessResponse("A reset token has been sent to your email");
        }

        public async Task<APIResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return APIResponse<object>.ErrorResponse("User not found");
            if (!request.NewPassword.Equals(request.ConfirmNewPassword))
                return APIResponse<object>.ErrorResponse("Passwords do not match");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return APIResponse<object>.ErrorResponse(errors, "Password reset failed");
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, _tokenOptions.Provider, TokenPurpose.ResetPassword.ToString());
            return APIResponse<object>.SuccessResponse("Password reset successful");
        }

        public async Task<APIResponse<object>> VerifyAuthToken(VerifyTokenRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return APIResponse<object>.ErrorResponse("User not found");

            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                _tokenOptions.Provider,
                request.Purpose.ToString(),
                request.Token
            );

            if (!isValid)
                return APIResponse<object>.ErrorResponse("Invalid or expired token");

            //await _userManager.RemoveAuthenticationTokenAsync(user, _tokenOptions.Provider, request.Purpose.ToString());

            return APIResponse<object>.SuccessResponse("Token is valid");
        }

        public async Task<APIResponse<object>> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return APIResponse<object>.ErrorResponse("User not found.");
            if (await _userManager.IsEmailConfirmedAsync(user))
                return APIResponse<object>.ErrorResponse("You have already confirmed your email.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return APIResponse<object>.ErrorResponse(errors, "Email confirmation failed.");
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, _tokenOptions.Provider, TokenPurpose.EmailConfirmation.ToString());
            return APIResponse<object>.SuccessResponse("Email confirmed successfully.");
        }

        public async Task<APIResponse<object>> SendEmailConfirmationTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return APIResponse<object>.ErrorResponse("User not found");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return APIResponse<object>.SuccessResponse("Your email is already confirmed.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (user.Email == null)
                return APIResponse<object>.ErrorResponse("User email is null");

            try {
                var emailConfirmRequest = new EmailVerificationCodeRequest
                {
                    Recipient = user.Email,
                    DisplayName = user.DisplayName ?? user.Email,
                    BodyPurpose = EmailTemplates.EmailConfirmationBodyPurpose,
                    Code = token
                };
                await _emailService.SendVerificationCodeEmailAsync(emailConfirmRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't send Email Confirmation verification code. Exception: {ex.Message}");
            }

            return APIResponse<object>.SuccessResponse("A confirmation code has been sent to your email.");
        }

    }
}
