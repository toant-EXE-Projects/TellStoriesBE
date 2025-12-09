using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigrationController : ControllerBase
    {
        private readonly IMigrationService _migrationService;

        public MigrationController(
            IMigrationService migrationService
        )
        {
            _migrationService = migrationService;
        }

        [HttpPost("user/all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> MigrateAllUsers(CancellationToken ct = default)
        {
            try
            {
                var count = await _migrationService.MigrateAllUsersAsync(ct);
                return Ok(APIResponse<object>.SuccessResponse(
                    message: $"{count} users migrated successfully."
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse(
                    message: $"Migration Failed {ex.Message}"
                ));
            }
        }

        [HttpPost("user/{userId}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> MigrateUser(string userId, CancellationToken ct = default)
        {
            var success = await _migrationService.MigrateUserAsync(userId, ct);
            if (!success)
            {
                return NotFound(APIResponse<object>.ErrorResponse(
                    message: $"User with ID {userId} not found."
                ));
            }

            return Ok(APIResponse<object>.SuccessResponse(
                message: $"User {userId} migrated successfully."
            ));
        }

        [HttpPost("story/story-type")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> MigrateStoryType(int type, CancellationToken ct = default)
        {
            try
            {
                var count = await _migrationService.MigrateStoryTypesAsync(type, ct);
                return Ok(APIResponse<object>.SuccessResponse(
                    message: $"{count} stories migrated successfully."
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse(
                    message: $"Migration Failed {ex.Message}"
                ));
            }
        }
    }
}
