using Microsoft.EntityFrameworkCore;
using UserManagementOtpVerfiyApp.Data;
using UserManagementOtpVerfiyApp.Models;

namespace UserManagementOtpVerfiyApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) { _db = db; }

        // Implement repository methods here
        public async Task<User> CraeteAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetByMobileAsync(string mobile) =>
          await  _db.Users.FirstOrDefaultAsync(a => a.MobileNumber == mobile);

        public async Task<User> UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
