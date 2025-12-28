using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Entities;
using UserManagementAPI.Modells;

namespace UserManagementAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        UserDbContext _context;
        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            var userEntity = new UserEntity(user.Id, user.PasswordHash, user.Email, user.Username);

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmail(string email)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            return User.Create(user.Id, user.PasswordHash, user.Email, user.Username);
        }

        public async Task<User?> GetByUsername(string username)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }
            return User.Create(user.Id, user.PasswordHash, user.Email, user.Username);
        }

        public async Task<User?> GetByUserId(string userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return null;
            }
            return User.Create(user.Id, user.PasswordHash, user.Email, user.Username);
        }
    }
}
