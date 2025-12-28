using UserManagementAPI.Entities;
using UserManagementAPI.Modells;

namespace UserManagementAPI.Repositories
{
    public interface IUserRepository
    {
        Task Add(User user);
        Task<User?> GetByEmail(string email);
        Task<User?> GetByUsername(string email);
        Task<User?> GetByUserId(string email);
    }
}