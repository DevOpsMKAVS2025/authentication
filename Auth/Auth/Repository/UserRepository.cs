using Auth.Data;
using Auth.Models;
using Auth.Repository.Interface;
using Auth.Repository.Utils;
using Microsoft.EntityFrameworkCore;

namespace Auth.Repository
{
    public class UserRepository : CrudRepository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context) {}

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

    }
}
