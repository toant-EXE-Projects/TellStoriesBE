using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public EmailController(IEmailService emailService, IMapper mapper)
        {
            _emailService = emailService;
            _mapper = mapper;
        }

        [HttpPost("send")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SendEmail([FromBody] EmailSendRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid email request");

            try
            {
                await _emailService.SendEmailAsync(request.Recipients, request.Subject, request.Body, request.ccRecipients, request.bccRecipients);
                return Ok(APIResponse<object>.SuccessResponse("Email sent successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse($"An unexpected error occurred: {ex}"));
            }
        }

        [HttpPost("send-receipt")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SendReceiptEmail([FromBody] EmailSubscriptionReceiptRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid email request");

            try
            {
                await _emailService.SendSubscriptionReceiptEmailAsync(request);
                return Ok(APIResponse<object>.SuccessResponse("Email sent successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse($"An unexpected error occurred: {ex}"));
            }
        }

        [HttpPost("send-subscription-reminder")]
        [Authorize(Policy = Policies.StaffOnly)]
        public async Task<IActionResult> SendSubscriptionReminderEmail([FromBody] EmailSubscriptionReminderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid email request");

            try
            {
                await _emailService.SendSubscriptionReminderEmailAsync(request);
                return Ok(APIResponse<object>.SuccessResponse("Email sent successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<object>.ErrorResponse($"An unexpected error occurred: {ex}"));
            }
        }

        //[HttpPost("send-verification")]
        //[Authorize(Policy = Policies.StaffOnly)]
        //public async Task<IActionResult> SendVerification([FromBody] EmailVerificationCodeRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest("Invalid email request");

        //    try
        //    {
        //        await _emailService.SendVerificationCodeEmailAsync(request);
        //        return Ok(APIResponse<object>.SuccessResponse("Email sent successfully."));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, APIResponse<object>.ErrorResponse($"An unexpected error occurred: {ex}"));
        //    }
        //}
    }
}
