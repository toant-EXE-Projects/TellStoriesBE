using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoryTeller.API.Utils;
using StoryTeller.Data.Base;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.ResponseModel;
using VNPAY.NET.Utilities;

namespace StoryTeller.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VNPayController : ControllerBase
    {
        private readonly IVNPayService _VNPayService;
        private readonly IDateTimeProvider _dateTimeService;
        private readonly IUserContextService _userContext;
        private readonly string? _mobileDeepLink;
        private readonly IConfiguration _config;

        public VNPayController(IVNPayService VNPayService,
            IDateTimeProvider dateTimeService,
            IUserContextService userContext,
            IConfiguration config
        )
        {
            _dateTimeService = dateTimeService;
            _VNPayService = VNPayService;
            _userContext = userContext;
            _config = config;

            _mobileDeepLink = _config["CallBack:MobileDeepLink"];
        }

        [HttpGet("create-payment-url")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentUrl(Guid subscriptionId)
        {
            var user = await _userContext.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized(APIResponse<object>.ErrorResponse("User not found"));
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var paymentUrl = await _VNPayService.CreateRequestUrl(ipAddress, subscriptionId, user);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
            }
        }

        //https://sandbox.vnpayment.vn/vnpaygw-sit-testing/user/login
        [HttpGet("ipn-action")]
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _VNPayService.Vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        return Ok();
                    }

                    return BadRequest(APIResponse<object>.ErrorResponse("Payment Failed"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound(APIResponse<object>.ErrorResponse("Payment Not Found."));
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            var userAgent = Request.Headers["User-Agent"].ToString().ToLower();
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var query = Request.Query;
                    var queryString = Request.QueryString.Value ?? "";

                    var paymentResult = _VNPayService.Vnpay.GetPaymentResult(query);
                    var resultDescription = $"PaymentResponse: {paymentResult.PaymentResponse.Description}. TransactionStatus:{paymentResult.TransactionStatus.Description}.";
                    var resultDesc = new
                    {
                        PaymentResponse = paymentResult.PaymentResponse.Description,
                        TransactionStatus = paymentResult.TransactionStatus.Description,
                        PaymentResult = paymentResult,
                    };

                    string redirectUrl;

                    if (paymentResult.IsSuccess)
                    {
                        var sanityCheck = await _VNPayService.CheckOrderAndSubscribeOrExtendAsync(
                            paymentResult
                        );

                        if (HttpHelper.IsMobileDevice(userAgent))
                        {
                            redirectUrl = $"{_mobileDeepLink}{queryString}";
                            return Redirect(redirectUrl);
                        }
                        else
                        {
                            return Ok(APIResponse<object>.SuccessResponse(resultDesc));
                        }
                    }

                    if (HttpHelper.IsMobileDevice(userAgent))
                    {
                        redirectUrl = $"{_mobileDeepLink}{queryString}";
                        return Redirect(redirectUrl);
                    }
                    else
                    {
                        return BadRequest(APIResponse<object>.ErrorResponse(resultDescription));
                    }
                }
                catch (Exception ex)
                {

                    var error = Uri.EscapeDataString(ex.Message);
                    if (HttpHelper.IsMobileDevice(userAgent))
                    {
                        var errorLink = $"{_mobileDeepLink}?error={error}";
                        return Redirect(errorLink);
                    }
                    return BadRequest(APIResponse<object>.ErrorResponse(ex.Message));
                }
            }

            return NotFound(APIResponse<object>.ErrorResponse("Payment Info Not Found."));
        }
    }
}
