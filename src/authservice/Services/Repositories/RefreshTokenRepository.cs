using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Entities;

namespace UserManagementAPI.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        UserDbContext _context;
        public RefreshTokenRepository(UserDbContext context)
        {
            _context = context;
        }

        public async Task Add(RefreshTokenEntity token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshTokenEntity?> GetByToken(string token)
        {
            var refreshToken = await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == Guid.Parse(token));
            if (refreshToken == null)
            {
                return null;
            }
            return refreshToken;
        }

        public async Task Delete(RefreshTokenEntity token)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }
}
