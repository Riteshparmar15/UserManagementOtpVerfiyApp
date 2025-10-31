<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
=======
﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementOtpVerfiyApp.Data;
>>>>>>> main
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
<<<<<<< HEAD
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserRepository repo, IOtpService otpService, IJwtService jwtService, IConfiguration config, ILogger<AuthController> logger)
=======
        public readonly IEmailOtpServices _emailOtpServices;
        private readonly ILogger<AuthController> _logger;
        private readonly AppDbContext _db;

        public AuthController(IUserRepository repo, IOtpService otpService, IJwtService jwtService, IConfiguration config, ILogger<AuthController> logger, AppDbContext db, IEmailOtpServices emailOtpServices)
>>>>>>> main
        {
            _repo = repo;
            _otpService = otpService;
            _jwtService = jwtService;
            _config = config;
            _logger = logger;
<<<<<<< HEAD
        }

=======
            _db = db;
            _emailOtpServices = emailOtpServices;
        }


        // Email registration
        // Register user with Email for google OAuth
        [HttpPost("Google/register")]
        public async Task<IActionResult> EmailRegisterGoogle([FromBody] RegisterRequest request)
        {
            if (request.Email == null || request.Password == null)
            {
                return BadRequest("Email and Password are required.");
            }
            else
            {
                var existingEmail = await _db.GoogleEmails.AnyAsync(x => x.Email == request.Email);
                if (existingEmail)
                {
                    return BadRequest("Email already registered.");
                }

                var googleEmail = new UserGoogleEmail
                {
                    Email = request.Email,
                    PasswordHash = request.Password // In production, hash the password before storing
                };

                _db.GoogleEmails.Add(googleEmail);
                await _db.SaveChangesAsync();
                return Ok(new { Message = "Email registered successfully for Google OAuth." });
            }
        }


        [HttpPost("Google/Login")]
        public async Task<IActionResult> LoginGoogleEmail([FromBody] LoginRequest request)
        {
            if (request.Email == null || request.Password == null)
            {
                return BadRequest("Email and Password are required.");
            }
            else
            {
                var user = await _db.GoogleEmails.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || user.PasswordHash != request.Password)
                    return Unauthorized(new { message = "Invalid email or password" });

                await _emailOtpServices.SendOtpAsync(user.Email);
                return Ok(new { message = "OTP sent to your email" });
            }
        }


        [HttpPost("Google/verify-otp")]
        public async Task<IActionResult> GoogleOtpVerify([FromBody] VerifyOtpRequest request)
        {
            var result = await _emailOtpServices.VerifyOtpAsync(request.Email, request.Otp);

            if (!result)
                return BadRequest(new { message = "Invalid OTP or OTP has expired" });

            return Ok(new { message = "OTP verified successfully" });
        }


        /// <summary>
        /// ////////////
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>


>>>>>>> main
        [HttpPost("Auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.MobileNumber))
                return BadRequest("Mobile number is required.");

            // Check if user already exists
            var existingUser = await _repo.GetByMobileAsync(dto.MobileNumber);
<<<<<<< HEAD
            if (existingUser != null) 
=======
            if (existingUser != null)
>>>>>>> main
            {
                return BadRequest("User with this mobile number already exists.");
            }

            // Create new user 
<<<<<<< HEAD
            var user = new User{ MobileNumber = dto.MobileNumber };
=======
            var user = new User { MobileNumber = dto.MobileNumber };
>>>>>>> main

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
<<<<<<< HEAD
                return NotFound("User not found.");
=======
            {
                user = new User { MobileNumber = dto.MobileNumber };
                await _repo.CraeteAsync(user);
            }

            // Prevent spam: active OTP Exists
            if (user.OtpExpiry != null && DateTime.UtcNow < user.OtpExpiry)
            {
                var waitSeconds = (user.OtpExpiry.Value - DateTime.UtcNow).TotalSeconds;
                return BadRequest($"An active OTP already exists. Please wait {Math.Ceiling(waitSeconds)} seconds before requesting a new OTP.");
            }
>>>>>>> main

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
<<<<<<< HEAD
            var returnOtp = _config.GetValue<bool>("Otp:ReturnInResponse",false);
=======
            var returnOtp = _config.GetValue<bool>("Otp:ReturnInResponse", false);
>>>>>>> main

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
<<<<<<< HEAD
            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                return BadRequest("OTP has expired, Request a new Otp.");
=======
            if (user.OtpExpiry == null)
                return BadRequest("No OTP found. Please request a new OTP.");

            if (DateTime.UtcNow > user.OtpExpiry)
            {
                user.Otp = null;
                user.OtpExpiry = null;
                await _repo.UpdateAsync(user);
                return BadRequest("OTP has expired. Please request a new OTP.");
            }
>>>>>>> main

            // Check OTP expiry
            if (user.Otp != dto.Otp)
                return BadRequest("Invalid OTP.");

<<<<<<< HEAD
=======
            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user.MobileNumber, out var expiresAt);

>>>>>>> main
            user.Otp = null;
            user.OtpExpiry = null;
            await _repo.UpdateAsync(user);

<<<<<<< HEAD
            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user.MobileNumber, out var expiresAt);
            return Ok(new { Message = "OTP verified successfully.", Token = token, ExpiresAt = expiresAt });
        }

=======
            return Ok(new { Message = "OTP verified successfully.", Token = token, ExpiresAt = expiresAt });
        }




>>>>>>> main
    }
}
