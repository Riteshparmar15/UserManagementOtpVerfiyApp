
namespace UserManagementOtpVerfiyApp.Services
{
    // This is a debug implementation of ISmsSender that does not actually send SMS messages.
    // This prints OTP to logs for local testing
    public class DebugSmsSender : ISmsSender
    {
        // for logging the OTP message
        private readonly ILogger<DebugSmsSender> _logger;
        public DebugSmsSender(ILogger<DebugSmsSender> logger )
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string mobileNumber, string message)
        {
            _logger.LogInformation("Debug SMS to {MobileNumber}: {Message}", mobileNumber, message);
            return Task.CompletedTask;
        }
    }
}
