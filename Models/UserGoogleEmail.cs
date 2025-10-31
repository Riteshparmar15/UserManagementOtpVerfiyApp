namespace UserManagementOtpVerfiyApp.Models
{
    public class UserGoogleEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
