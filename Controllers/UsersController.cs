using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Messaging;

namespace UserManagementOtpVerfiyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            // ✅ Get the mobile number from JWT claims
            var mobile = User.FindFirst("mobileNumber")?.Value;

            if (string.IsNullOrEmpty(mobile))
                return Unauthorized(new { Message = "Mobile number claim missing or invalid token" });

            return Ok(new
            {
                Mobile = mobile,
                Message = "Protected endpoint - you are authenticated"
            });
        }
    }
}
