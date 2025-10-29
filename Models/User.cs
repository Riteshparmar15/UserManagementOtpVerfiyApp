namespace UserManagementOtpVerfiyApp.Models
{
    public class User
    {
        public int Id { get; set; }
        // User Name 
        public string UserName { get; set; } = string.Empty;

        // Unique mobile number
        public string MobileNumber { get; set; } = null!;
        // We'll store OTP and expiry for simplicity (in production: store OTP hash or use short-lived cache)
        public string? Otp { get; set; }
        // OTP expiry time
        public DateTime? OtpExpiry { get; set; }
        // Optional metadata
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
