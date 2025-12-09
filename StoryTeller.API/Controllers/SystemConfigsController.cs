using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Admin)]
    public class SystemConfigsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISystemConfigurationService _sysConfigService;
        private readonly IUserContextService _userContext;

        public SystemConfigsController(
            IMapper mapper, 
            ISystemConfigurationService sysConfigService,
            IUserContextService userContext)
        {
            _mapper = mapper;
            _sysConfigService = sysConfigService;
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var configs = await _sysConfigService.GetAllConfigsAsync(ct);
            //var l_dr_p = await _sysConfigService.GetIntAsync(SystemConst.Keys.Login_DailyReward_Points);
            //var sp_mpr_d = await _sysConfigService.GetIntAsync(SystemConst.Keys.StoryPublish_MaxPendingRequests_Default);
            //var sp_mpr_t1 = await _sysConfigService.GetIntAsync(SystemConst.Keys.StoryPublish_MaxPendingRequests_Tier1);
            //var s_bg_ce_im = await _sysConfigService.GetIntAsync(SystemConst.Keys.Subscription_Background_CheckExpiration_IntervalMinutes);
            //var s_bg_seue_im = await _sysConfigService.GetIntAsync(SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_IntervalMinutes);
            //var s_bg_seue_dbe = await _sysConfigService.GetIntAsync(SystemConst.Keys.Subscription_Background_SendEmailUpcomingExpiration_DaysBeforeExpiration);
            //var br_bg_cr_im = await _sysConfigService.GetIntAsync(SystemConst.Keys.BillingRecord_Background_CheckRecords_IntervalMinutes);
            //var br_bg_mrafinpim = await _sysConfigService.GetIntAsync(SystemConst.Keys.BillingRecord_Background_MarkRecordAsFailedIfNotPaidInMinutes);
            return Ok(
                APIResponse<object>.SuccessResponse(configs)
            //new {
            //    configs,
            //    l_dr_p,
            //    sp_mpr_d,
            //    sp_mpr_t1,
            //    s_bg_ce_im,
            //    s_bg_seue_im,
            //    s_bg_seue_dbe,
            //    br_bg_cr_im,
            //    br_bg_mrafinpim,
            //}
            );
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] SystemConfigUpdateRequest request, CancellationToken ct)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));

            var value = await _sysConfigService.GetValueAsync(key, ct);
            if (value == null)
                return BadRequest(APIResponse<object>.ErrorResponse($"Key: [{key}] Does not exist."));

            await _sysConfigService.UpdateAsync(key, request.Value, user, ct);
            return Ok(APIResponse<object>.SuccessResponse($"Key: [{key}] Old: {value} => New: {request.Value}"));
        }

        [HttpPost("refresh-cache")]
        public async Task<IActionResult> RefreshCache(CancellationToken ct)
        {
            await _sysConfigService.RefreshCacheAsync(ct);
            return Ok(APIResponse<object>.SuccessResponse("System Config Cache Refreshed."));
        }
    }
}
