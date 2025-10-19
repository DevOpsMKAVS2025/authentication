using Auth.Models;

namespace Auth.Repository
{
    public interface IUserRepository : ICrudRepository<User>
    {
        Task<User?> GetUserByEmail(string email);
    }
}
