using Microsoft.EntityFrameworkCore;
using UserManagementOtpVerfiyApp.Data;
using UserManagementOtpVerfiyApp.Models;

namespace UserManagementOtpVerfiyApp.Services
{

    public interface IEmailOtpServices
    {
        Task<bool> SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otp);
    }
    public class EmailOtpServices : IEmailOtpServices
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        public EmailOtpServices(AppDbContext db, IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000,999999).ToString();
        }


        public async Task<bool> SendOtpAsync(string email)
        {
            var otp = GenerateOtp();
            var expiry = DateTime.UtcNow.AddMinutes(2);

            var otpEmtity = new EmailOtp
            {
                Email = email,
                OtpCode = otp,
                ExpiryTime = expiry
            };

            _db.EmailOtps.Add(otpEmtity);
            await _db.SaveChangesAsync();

            string subject = "Your OTP Code";
            string body = $"Your OTP code is {otp}. It will expire in 2 minutes.";

            await _emailSender.SendEmailAsync(email, subject, body);
            return true;
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var record = await _db.EmailOtps
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsVerified);

            if (record == null || record.ExpiryTime < DateTime.UtcNow || record.OtpCode != otp)
                return false;

            record.IsVerified = true;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
