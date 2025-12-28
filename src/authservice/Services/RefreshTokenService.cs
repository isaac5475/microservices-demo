using UserManagementAPI.Entities;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Services
{
    public class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RefreshTokenEntity?> GetByToken(string token)
        {
            return await _refreshTokenRepository.GetByToken(token);
        }

        public async Task Delete(RefreshTokenEntity token)
        {
            await _refreshTokenRepository.Delete(token);
        }

        public async Task Save(RefreshTokenEntity token)
        {
            await _refreshTokenRepository.Add(token);
        }
    }
}
