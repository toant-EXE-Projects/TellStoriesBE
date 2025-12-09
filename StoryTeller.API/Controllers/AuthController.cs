using Microsoft.AspNetCore.Mvc;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserInitializationService _userInitService;

        public AuthController(IAuthService authService, IUserInitializationService userInitService)
        {
            _authService = authService;
            _userInitService = userInitService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request);
            if (token == null)
                return Unauthorized(
                    APIResponse<object>.ErrorResponse(message: "Email or password is incorrect.")
                );

            return Ok(APIResponse<string>.SuccessResponse(token, "Login successful."));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var userId = await _authService.RegisterAsync(request);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(APIResponse<object>.ErrorResponse(
                    message: "User already exists or registration failed."
                ));
            }

            var initSuccess = await _userInitService.InitializeAsync(userId);

            return Ok(APIResponse<object>.SuccessResponse(
                message: initSuccess
                    ? "Registration successful."
                    : "Registration successful but initialization failed."
            ));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<object>.ErrorResponse("Invalid email"));


            var response = await _authService.ForgotPasswordAsync(request.Email);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(APIResponse<object>.ErrorResponse("Invalid reset request"));

            var response = await _authService.ResetPasswordAsync(request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation([FromBody] EmailOnlyRequest request)
        {
            var response = await _authService.SendEmailConfirmationTokenAsync(request.Email);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request.Email, request.Token);
            return Ok(result);
        }


        /// <summary>
        /// Verify Auth Token
        /// </summary>
        /// <remarks>
        /// ["purpose"] MUST be one of the following:
        ///     "EmailConfirmation"
        ///     | "ResetPassword"
        ///     | "TwoFactorAuth"
        ///     
        /// Sample:
        /// 
        ///     POST api/Auth/verify-token
        ///     {        
        ///       "Email": "user@example.com",
        ///       "token": "ABCDEF",
        ///       "purpose": "ResetPassword"
        ///     }
        /// </remarks>
        /// <param name="request"></param>
        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken(VerifyTokenRequest request)
        {
            var result = await _authService.VerifyAuthToken(request);
            return Ok(result);
        }
    }
}
