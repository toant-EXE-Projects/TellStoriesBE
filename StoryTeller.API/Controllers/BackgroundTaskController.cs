using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Background;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class BackgroundTaskController : ControllerBase
    {
        private readonly IBackgroundTaskToggle _toggle;

        public BackgroundTaskController(IBackgroundTaskToggle toggle)
        {
            _toggle = toggle;
        }

        [HttpPost("toggle/{taskType}")]
        public IActionResult ToggleTask(BackgroundTaskType taskType)
        {
            var _taskEnabled = _toggle.IsEnabled(taskType);
            if (_taskEnabled)
            {
                _toggle.SetEnabled(taskType, false);
            }
            else
            {
                _toggle.SetEnabled(taskType, true);
            }
            return Ok(APIResponse<object>.SuccessResponse($"{taskType}: {_toggle.IsEnabled(taskType)}."));
        }

        [HttpPost("enable/{taskType}")]
        public IActionResult EnableTask(BackgroundTaskType taskType)
        {
            _toggle.SetEnabled(taskType, true);
            return Ok(APIResponse<object>.SuccessResponse($"{taskType} task enabled."));
        }

        [HttpPost("disable/{taskType}")]
        public IActionResult DisableTask(BackgroundTaskType taskType)
        {
            _toggle.SetEnabled(taskType, false);
            return Ok(APIResponse<object>.SuccessResponse($"{taskType} task disabled."));
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var res = _toggle.GetAllTasks();
            return Ok(APIResponse<object>.SuccessResponse(res, "Background tasks."));
        }
    }
}
