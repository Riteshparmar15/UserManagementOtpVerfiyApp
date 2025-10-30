namespace UserManagementOtpVerfiyApp.Services
{
    public class OtpService : IOtpService
    {
        private readonly ISmsSender _smsSender;
        private readonly IConfiguration _config;
        public OtpService(ISmsSender smsSender, IConfiguration config) { _smsSender = smsSender; _config = config; }

        public string GenerateOtp()
        {
            var rnd = new Random();
            return rnd.Next(100000,999999).ToString("D6"); // 6 digit OTP
        }

        public Task SendOtpAsync(string mobileNumber, string otp)
        {
            var msgTemplate = _config["Otp:MessageTemplate"] ?? "Your OTP is {otp}. It will expire in {expirySeconds} seconds.";
            var expirySec = _config["Otp:ExpirySeconds"] ?? "15";
            var message = msgTemplate.Replace("{otp}", otp).Replace("{expirySeconds}", expirySec);
            return _smsSender.SendSmsAsync(mobileNumber, message);
        }
    }
}
