using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Data.Base;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Validator _validator;
        private readonly IDateTimeProvider _dateTimeService;

        public TestController(IMapper mapper, 
            Validator validator,
            IDateTimeProvider dateTimeService)
        {
            _mapper = mapper;
            _validator = validator;
            _dateTimeService = dateTimeService;
        }

        [HttpGet("GetTime")]
        public async Task<IActionResult> GetTime()
        {
            var t = new Dictionary<string, DateTime>() {
                { "UTC", _dateTimeService.GetUtcNow() },
                { "VNTime", _dateTimeService.GetSystemCurrentTime() },
                { "Asia/Ho_Chi_Minh", _dateTimeService.GetCurrentTime("Asia/Ho_Chi_Minh") }
            };

            return Ok(APIResponse<object>.SuccessResponse(t));
        }
        [HttpGet("test-vnpay")]
        public async Task<IActionResult> TestVnPayConnectivity()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");
                return Ok($"Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
