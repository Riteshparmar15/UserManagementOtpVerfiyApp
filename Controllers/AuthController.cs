using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementOtpVerfiyApp.Dtos;
using UserManagementOtpVerfiyApp.Models;
using UserManagementOtpVerfiyApp.Repositories;
using UserManagementOtpVerfiyApp.Services;

namespace UserManagementOtpVerfiyApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwtService;
        private readonly IOtpService _otpService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository repo, IOtpService otpService, IJwtService jwtService, IConfiguration config, ILogger<AuthController> logger)
        {
            _repo = repo;
            _otpService = otpService;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
        }

        [HttpPost("Auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.MobileNumber))
                return BadRequest("Mobile number is required.");

            // Check if user already exists
            var existingUser = await _repo.GetByMobileAsync(dto.MobileNumber);
            if (existingUser != null) 
            {
                return BadRequest("User with this mobile number already exists.");
            }

            // Create new user 
            var user = new User{ MobileNumber = dto.MobileNumber };

            // Save user to database
            await _repo.CraeteAsync(user);

            // Return success response
            return Ok(new { Message = "User registered successfully." });
        }

        [HttpPost("Auth/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.MobileNumber))
                return BadRequest("Mobile number is required.");

            // Find user by mobile number
            var user = await _repo.GetByMobileAsync(dto.MobileNumber);
            if (user == null)
                return NotFound("User not found.");

            // Generate OTP
            var otp = _otpService.GenerateOtp();
            user.Otp = otp;

            // Expire in configured seconds (15)
            var expirySeconds = int.TryParse(_config["Otp:ExpirySeconds"], out var s) ? s : 15;
            user.OtpExpiry = DateTime.UtcNow.AddSeconds(expirySeconds);

            // Update user with OTP
            await _repo.UpdateAsync(user);

            // Send OTP via SMS
            await _otpService.SendOtpAsync(user.MobileNumber, otp);

            // Return success response
            var returnOtp = _config.GetValue<bool>("Otp:ReturnInResponse",false);

            // return response
            return returnOtp ? Ok(new { Message = "OTP sent successfully.", Otp = otp, ExpiresAt = user.OtpExpiry })
                             : Ok(new { Message = "OTP sent." });
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.MobileNumber) || string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest("Mobile number and OTP are required.");

            // Find user by mobile number
            var user = await _repo.GetByMobileAsync(dto.MobileNumber);
            if (user == null) return NotFound("User not found.");

            // Check OTP expiry
            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                return BadRequest("OTP has expired, Request a new Otp.");

            // Check OTP expiry
            if (user.Otp != dto.Otp)
                return BadRequest("Invalid OTP.");

            user.Otp = null;
            user.OtpExpiry = null;
            await _repo.UpdateAsync(user);

            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user.MobileNumber, out var expiresAt);
            return Ok(new { Message = "OTP verified successfully.", Token = token, ExpiresAt = expiresAt });
        }

    }
}
