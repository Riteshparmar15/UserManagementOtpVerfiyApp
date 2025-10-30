namespace UserManagementOtpVerfiyApp.Services
{
    public interface IOtpService
    {
        string GenerateOtp();
        Task SendOtpAsync(string mobileNumber, string otp);
    }
}
