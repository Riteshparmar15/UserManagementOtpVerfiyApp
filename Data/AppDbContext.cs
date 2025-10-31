using Microsoft.EntityFrameworkCore;
using UserManagementOtpVerfiyApp.Models;

namespace UserManagementOtpVerfiyApp.Data
{
    public class AppDbContext : DbContext   
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;

        public  DbSet<UserGoogleEmail> GoogleEmails { get; set; }

        public DbSet<EmailOtp> EmailOtps { get; set; }
    }
}
