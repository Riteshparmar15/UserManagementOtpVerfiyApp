using UserManagementOtpVerfiyApp.Models;

namespace UserManagementOtpVerfiyApp.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByMobileAsync(string mobile);
        Task<User> CraeteAsync(User user);
        Task<User> UpdateAsync(User user);
    } 
}
