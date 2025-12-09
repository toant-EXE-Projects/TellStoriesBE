using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.ResponseModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace StoryTeller.API.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                if (HasBearerToken(context))
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                    var user = await GetUserFromTokenAsync(context, userService);
                    if (user == null)
                        throw new Exception("User does not exist.");

                    if (!IsUserActive(user))
                        throw new Exception("Account is disabled or deleted. Please contact the administrator if this is an error.");

                    // Attach to HttpContext for later use
                    context.Items["User"] = user;
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static bool HasBearerToken(HttpContext context)
        {
            return context.Request.Headers.TryGetValue("Authorization", out var authHeader)
                && authHeader.ToString().StartsWith("Bearer ");
        }

        private static string GetToken(HttpContext context)
        {
            return context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }

        private static async Task<UserDTO?> GetUserFromTokenAsync(HttpContext context, IUserService userService)
        {
            var token = GetToken(context);
            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(token))
                throw new ArgumentException("Invalid JWT token.");

            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            var userId = jwtToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception("Invalid token payload.");

            return await userService.GetByIdAsync(userId);
        }

        private static bool IsUserActive(UserDTO user)
        {
            if (user.IsDeleted == true) return false;
            if (string.IsNullOrWhiteSpace(user.Status))
                return false;

            switch (user.Status)
            {
                case UserStatusConstants.Disabled:
                case UserStatusConstants.Inactive:
                case UserStatusConstants.Suspended:
                case UserStatusConstants.Banned:
                case UserStatusConstants.Deleted:
                    return false;
            }

            return user.Status == UserStatusConstants.Active;
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var result = JsonSerializer.Serialize(new APIResponse<string>
            {
                Message = "Internal Server Error",
                Errors = new List<string> { ex.Message }
            });

            await context.Response.WriteAsync(result);
        }
    }

    public static class AuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}