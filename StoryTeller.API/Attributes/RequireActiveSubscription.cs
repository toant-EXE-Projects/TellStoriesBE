using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.ResponseModel;
using System.Security.Claims;

namespace StoryTeller.Services.Attributes
{
    public class RequireActiveSubscriptionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Result = new ObjectResult(
                    APIResponse<object>.ErrorResponse("Unauthorized")
                )
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            if (Roles.Staff.Any(role => user.IsInRole(role)))
            {
                return;
            }

            var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();

            var isSubscribed = await subscriptionService.GetUserActiveSubscriptionAsync(userId);

            if (isSubscribed == null)
            {
                context.Result = new ObjectResult(
                    APIResponse<object>.ErrorResponse(
                        errors: new List<string> { "You must have an active subscription to access this resource." },
                        message: "Forbidden"
                    )
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
