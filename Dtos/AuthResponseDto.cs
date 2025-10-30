namespace UserManagementOtpVerfiyApp.Dtos
{
    public record AuthResponseDto(string Token, DateTime ExpiresAt);
}
