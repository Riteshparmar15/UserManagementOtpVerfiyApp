
using System.Net;
using System.Net.Mail;

namespace UserManagementOtpVerfiyApp.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toemail, string subject, string body)
        {
            var fromEmail = _config["EmailSettings:From"];
            var password = _config["EmailSettings:Password"];
            var smtpHost = _config["EmailSettings:SmtpHost"];
            var smtpPort = _config["EmailSettings:SmtpPort"];
            int port = int.Parse(smtpPort);

            using var client = new SmtpClient(smtpHost, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail,password)
            };

            var email = new MailMessage(fromEmail, toemail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(email);
        }
    }
}
