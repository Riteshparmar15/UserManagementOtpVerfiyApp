namespace UserManagementOtpVerfiyApp.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(string mobileNumber, out DateTime expireAt);
    }
}
