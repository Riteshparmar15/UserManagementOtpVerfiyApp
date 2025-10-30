namespace UserManagementOtpVerfiyApp.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string mobileNumber, string message);
    }
}
