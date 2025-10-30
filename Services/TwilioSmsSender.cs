
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace UserManagementOtpVerfiyApp.Services
{
    public class TwilioSmsSender : ISmsSender
    {
        private readonly IConfiguration _config;
        public TwilioSmsSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendSmsAsync(string mobileNumber, string message)
        {
            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            var fromNumber = _config["Twilio:FromNumber"];
            TwilioClient.Init(accountSid, authToken);

            return MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(fromNumber),
                to: new Twilio.Types.PhoneNumber(mobileNumber)
            );
        }
    }
}
