using Auth.Dto;
using Auth.Models;

namespace Auth.Services
{
    public interface IAuthService
    {
        Task<Tokens> Login(LoginCredentialsDto credentials);
    }
}
