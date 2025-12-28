using UserManagementAPI.Entities;

namespace UserManagementAPI.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Add(RefreshTokenEntity token);
        Task<RefreshTokenEntity> GetByToken(string token);
        Task Delete(RefreshTokenEntity token);
    }
}