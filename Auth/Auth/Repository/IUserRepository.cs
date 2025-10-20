using Auth.Model;

namespace Auth.Repository
{
    public interface IUserRepository : ICrudRepository<User>
    {
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByUsername(string username);
    }
}
